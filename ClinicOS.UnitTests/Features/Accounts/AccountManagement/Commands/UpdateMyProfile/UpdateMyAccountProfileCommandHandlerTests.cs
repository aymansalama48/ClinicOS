using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.UpdateMyProfile;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.AccountManagement.Commands.UpdateMyProfile;

public class UpdateMyAccountProfileCommandHandlerTests
{
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<IUserManagementService> _userServiceMock;
    private readonly UpdateMyAccountProfileCommandHandler _handler;

    public UpdateMyAccountProfileCommandHandlerTests()
    {
        _currentUserMock = new Mock<ICurrentUser>();
        _userServiceMock = new Mock<IUserManagementService>();
        _handler = new UpdateMyAccountProfileCommandHandler(_currentUserMock.Object, _userServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_CurrentUser_Is_Invalid()
    {
        // Arrange
        _currentUserMock.Setup(u => u.UserId).Returns((Guid?)null);

        var command = new UpdateMyAccountProfileCommand("Ahmed", null, "Ali", "01012345678");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(UserErrors.NotFound);

        _userServiceMock.Verify(u => u.UpdateProfileAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_UserService_Fails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(u => u.UserId).Returns(userId);

        var error = new Error("Profile.Failed", "Failed to update profile", ErrorType.Validation);
        _userServiceMock.Setup(u => u.UpdateProfileAsync(userId, "Ahmed Ali", "01012345678", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(error));

        var command = new UpdateMyAccountProfileCommand("Ahmed", null, "Ali", "01012345678");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }

    [Fact]
    public async Task Handle_Should_Succeed_When_Valid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(u => u.UserId).Returns(userId);

        _userServiceMock.Setup(u => u.UpdateProfileAsync(userId, "Ahmed Mahmoud Ali", "01012345678", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var command = new UpdateMyAccountProfileCommand("Ahmed", "Mahmoud", "Ali", "01012345678");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
    }
}
