using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Receptionists;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Application.Features.Receptionists.Commands.CompleteMyProfile;
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

namespace ClinicOS.UnitTests.Features.Receptionists.Commands.CompleteMyProfile;

public class CompleteMyReceptionistProfileCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly CompleteMyReceptionistProfileCommandHandler _handler;

    public CompleteMyReceptionistProfileCommandHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _currentUserMock = new Mock<ICurrentUser>();
        _handler = new CompleteMyReceptionistProfileCommandHandler(_contextMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_CurrentUser_Has_No_Id()
    {
        // Arrange
        _currentUserMock.Setup(u => u.UserId).Returns((Guid?)null);
        var command = new CompleteMyReceptionistProfileCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(UserErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Profile_Already_Exists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(u => u.UserId).Returns(userId);

        var existing = Receptionist.Create(userId, Guid.NewGuid());
        var receptionists = new List<Receptionist> { existing }.AsQueryable();
        _contextMock.Setup(c => c.Receptionists).Returns(receptionists);

        var command = new CompleteMyReceptionistProfileCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(ReceptionistErrors.ProfileAlreadyExists);
    }

    [Fact]
    public async Task Handle_Should_Create_Profile_When_Valid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var specId = Guid.NewGuid();
        _currentUserMock.Setup(u => u.UserId).Returns(userId);

        var receptionists = new List<Receptionist>().AsQueryable();
        _contextMock.Setup(c => c.Receptionists).Returns(receptionists);

        var command = new CompleteMyReceptionistProfileCommand(specId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();

        _contextMock.Verify(c => c.Add(It.Is<Receptionist>(r => r.ApplicationUserId == userId && r.SpecializationId == specId)), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
