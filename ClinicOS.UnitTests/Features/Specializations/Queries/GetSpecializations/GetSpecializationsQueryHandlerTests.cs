using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Application.Features.Specializations.Queries.GetSpecializations;
using ClinicOS.Application.Features.Specializations.Shared;
using ClinicOS.Domain.Entities.Specializations;
using ClinicOS.UnitTests.Common;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Specializations.Queries.GetSpecializations;

public class GetSpecializationsQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly GetSpecializationsQueryHandler _handler;

    public GetSpecializationsQueryHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _contextMock.SetupQueryType<SpecializationResponse>();
        _handler = new GetSpecializationsQueryHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Paginated_Specializations_Without_Filter()
    {
        // Arrange
        var list = new List<Specialization>
        {
            new("Cardiology", "Heart"),
            new("Dentistry", "Teeth"),
            new("Neurology", "Brain")
        };
        _contextMock.Setup(c => c.Specializations).Returns(list.AsQueryable());

        var query = new GetSpecializationsQuery(null, new PaginationParameters { PageNumber = 1, PageSize = 10 });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(3);
        result.Data.Pagination.TotalCount.Should().Be(3);
        result.Data.Pagination.CurrentPage.Should().Be(1);
    }

    [Fact]
    public async Task Handle_Should_Filter_By_SearchTerm()
    {
        // Arrange
        var list = new List<Specialization>
        {
            new("Cardiology", "Heart"),
            new("Dentistry", "Teeth"),
            new("Dermatology", "Skin")
        };
        _contextMock.Setup(c => c.Specializations).Returns(list.AsQueryable());

        var query = new GetSpecializationsQuery("Dent", new PaginationParameters { PageNumber = 1, PageSize = 10 });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().Name.Should().Be("Dentistry");
        result.Data.Pagination.TotalCount.Should().Be(1);
    }
}
