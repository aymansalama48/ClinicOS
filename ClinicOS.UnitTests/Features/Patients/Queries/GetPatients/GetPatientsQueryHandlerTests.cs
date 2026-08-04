using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Application.Features.Patients.Queries.GetPatients;
using ClinicOS.Domain.Entities.Patients;
using ClinicOS.Domain.Enums;
using ClinicOS.UnitTests.Common;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Patients.Queries.GetPatients;

public class GetPatientsQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly GetPatientsQueryHandler _handler;

    public GetPatientsQueryHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _contextMock.SetupQueryType<PatientResponse>();
        _handler = new GetPatientsQueryHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Paginated_Patients()
    {
        // Arrange
        var p1 = new Patient { FirstName = "Ali", LastName = "Hassan", PhoneNumber = "0101" };
        var p2 = new Patient { FirstName = "Sara", LastName = "Kamal", PhoneNumber = "0102" };
        var patients = new List<Patient> { p1, p2 }.AsQueryable();
        _contextMock.Setup(c => c.Patients).Returns(patients);

        var query = new GetPatientsQuery(null, new PaginationParameters { PageNumber = 1, PageSize = 10 });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.Pagination.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_Should_Filter_By_SearchTerm()
    {
        // Arrange
        var p1 = new Patient { FirstName = "Ali", LastName = "Hassan", PhoneNumber = "01011111111" };
        var p2 = new Patient { FirstName = "Sara", LastName = "Kamal", PhoneNumber = "01022222222" };
        var patients = new List<Patient> { p1, p2 }.AsQueryable();
        _contextMock.Setup(c => c.Patients).Returns(patients);

        var query = new GetPatientsQuery("Sara", new PaginationParameters { PageNumber = 1, PageSize = 10 });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().FullName.Should().Contain("Sara");
    }
}
