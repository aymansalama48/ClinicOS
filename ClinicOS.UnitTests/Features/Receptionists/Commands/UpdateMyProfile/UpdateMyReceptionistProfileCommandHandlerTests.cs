using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Receptionists;
using ClinicOS.Application.Features.Receptionists.Commands.UpdateMyProfile;
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

namespace ClinicOS.UnitTests.Features.Receptionists.Commands.UpdateMyProfile;

public class UpdateMyReceptionistProfileCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly UpdateMyReceptionistProfileCommandHandler _handler;

    public UpdateMyReceptionistProfileCommandHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _currentUserMock = new Mock<ICurrentUser>();
        _handler = new UpdateMyReceptionistProfileCommandHandler(_contextMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Profile_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(u => u.UserId).Returns(userId);

        var receptionists = new List<Receptionist>().AsQueryable();
        _contextMock.Setup(c => c.Receptionists).Returns(receptionists);

        var command = new UpdateMyReceptionistProfileCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(ReceptionistErrors.ProfileNotFound);

        _contextMock.Verify(c => c.Update(It.IsAny<Receptionist>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Update_Profile_When_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var oldSpecId = Guid.NewGuid();
        var newSpecId = Guid.NewGuid();
        _currentUserMock.Setup(u => u.UserId).Returns(userId);

        var existing = Receptionist.Create(userId, oldSpecId);
        var receptionists = new List<Receptionist> { existing }.AsQueryable();
        _contextMock.Setup(c => c.Receptionists).Returns(receptionists);

        var command = new UpdateMyReceptionistProfileCommand(newSpecId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(existing.Id);
        existing.SpecializationId.Should().Be(newSpecId);

        _contextMock.Verify(c => c.Update(existing), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
