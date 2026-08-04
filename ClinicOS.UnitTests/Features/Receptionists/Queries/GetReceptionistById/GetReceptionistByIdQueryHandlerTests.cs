using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Receptionists;
using ClinicOS.Application.Features.Receptionists.Queries.GetReceptionistById;
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

namespace ClinicOS.UnitTests.Features.Receptionists.Queries.GetReceptionistById;

public class GetReceptionistByIdQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly GetReceptionistByIdQueryHandler _handler;

    public GetReceptionistByIdQueryHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _contextMock.SetupQueryType<ReceptionistDetailsResponse>();
        _handler = new GetReceptionistByIdQueryHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Receptionist_Does_Not_Exist()
    {
        // Arrange
        var receptionists = new List<Receptionist>().AsQueryable();
        _contextMock.Setup(c => c.Receptionists).Returns(receptionists);

        var query = new GetReceptionistByIdQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(ReceptionistErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_Return_Details_When_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var specId = Guid.NewGuid();
        var receptionist = Receptionist.Create(userId, specId);
        var receptionists = new List<Receptionist> { receptionist }.AsQueryable();
        _contextMock.Setup(c => c.Receptionists).Returns(receptionists);

        var query = new GetReceptionistByIdQuery(receptionist.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(receptionist.Id);
        result.Data.SpecializationId.Should().Be(specId);
        result.Data.ApplicationUserId.Should().Be(userId);
        result.Data.IsActive.Should().BeTrue();
    }
}
