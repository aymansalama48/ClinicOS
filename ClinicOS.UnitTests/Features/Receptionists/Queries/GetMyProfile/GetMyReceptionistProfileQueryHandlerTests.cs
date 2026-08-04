using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Receptionists;
using ClinicOS.Application.Features.Receptionists.Queries.GetMyProfile;
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

namespace ClinicOS.UnitTests.Features.Receptionists.Queries.GetMyProfile;

public class GetMyReceptionistProfileQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly GetMyReceptionistProfileQueryHandler _handler;

    public GetMyReceptionistProfileQueryHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _currentUserMock = new Mock<ICurrentUser>();
        _handler = new GetMyReceptionistProfileQueryHandler(_contextMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Profile_Does_Not_Exist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(u => u.UserId).Returns(userId);

        var receptionists = new List<Receptionist>().AsQueryable();
        _contextMock.Setup(c => c.Receptionists).Returns(receptionists);

        var query = new GetMyReceptionistProfileQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(ReceptionistErrors.ProfileNotFound);
    }

    [Fact]
    public async Task Handle_Should_Return_Profile_When_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var specId = Guid.NewGuid();
        _currentUserMock.Setup(u => u.UserId).Returns(userId);

        var receptionist = Receptionist.Create(userId, specId);
        var receptionists = new List<Receptionist> { receptionist }.AsQueryable();
        _contextMock.Setup(c => c.Receptionists).Returns(receptionists);

        var query = new GetMyReceptionistProfileQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(receptionist.Id);
        result.Data.SpecializationId.Should().Be(specId);
        result.Data.IsActive.Should().BeTrue();
    }
}
