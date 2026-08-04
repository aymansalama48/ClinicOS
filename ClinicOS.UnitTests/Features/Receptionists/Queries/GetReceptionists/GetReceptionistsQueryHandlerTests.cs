using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Application.Features.Receptionists.Queries.GetReceptionists;
using ClinicOS.Domain.Entities.Receptionists;
using ClinicOS.UnitTests.Common;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Receptionists.Queries.GetReceptionists;

public class GetReceptionistsQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly GetReceptionistsQueryHandler _handler;

    public GetReceptionistsQueryHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _contextMock.SetupQueryType<ReceptionistResponse>();
        _handler = new GetReceptionistsQueryHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Paginated_Receptionists()
    {
        // Arrange
        var specId = Guid.NewGuid();
        var rec1 = Receptionist.Create(Guid.NewGuid(), specId);
        var rec2 = Receptionist.Create(Guid.NewGuid(), specId);
        var receptionists = new List<Receptionist> { rec1, rec2 }.AsQueryable();
        _contextMock.Setup(c => c.Receptionists).Returns(receptionists);

        var query = new GetReceptionistsQuery(null, null, new PaginationParameters { PageNumber = 1, PageSize = 10 });

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
    public async Task Handle_Should_Filter_By_SpecializationId_And_IsActive()
    {
        // Arrange
        var targetSpecId = Guid.NewGuid();
        var otherSpecId = Guid.NewGuid();

        var rec1 = Receptionist.Create(Guid.NewGuid(), targetSpecId);
        var rec2 = Receptionist.Create(Guid.NewGuid(), otherSpecId);
        var receptionists = new List<Receptionist> { rec1, rec2 }.AsQueryable();
        _contextMock.Setup(c => c.Receptionists).Returns(receptionists);

        var query = new GetReceptionistsQuery(targetSpecId, true, new PaginationParameters { PageNumber = 1, PageSize = 10 });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().SpecializationId.Should().Be(targetSpecId);
    }
}
