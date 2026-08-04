using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.DeactivateUser;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.AccountManagement.Commands.DeactivateUser;

public class DeactivateUserCommandHandlerTests
{
    private readonly Mock<IUserManagementService> _userServiceMock;
    private readonly Mock<IJobScheduler> _jobSchedulerMock;
    private readonly DeactivateUserCommandHandler _handler;

    public DeactivateUserCommandHandlerTests()
    {
        _userServiceMock = new Mock<IUserManagementService>();
        _jobSchedulerMock = new Mock<IJobScheduler>();

        _handler = new DeactivateUserCommandHandler(
            _userServiceMock.Object,
            _jobSchedulerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Deactivation_Fails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var error = new Error("User.DeactivationFailed", "Could not deactivate user", ErrorType.Validation);

        _userServiceMock.Setup(u => u.DeactivateUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(error));

        var command = new DeactivateUserCommand(userId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);

        _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _jobSchedulerMock.Verify(j => j.Enqueue(It.IsAny<Expression<Func<IIdentityNotificationService, Task>>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Succeed_And_Enqueue_Email_When_User_Has_Email()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userServiceMock.Setup(u => u.DeactivateUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var userDto = new UserDto
        {
            Id = userId,
            Email = "user@example.com",
            FullName = "Deactivated User"
        };

        _userServiceMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserDto>.Success(userDto));

        var command = new DeactivateUserCommand(userId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();

        _jobSchedulerMock.Verify(j => j.Enqueue(It.IsAny<Expression<Func<IIdentityNotificationService, Task>>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Succeed_Without_Email_When_User_Has_No_Email()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userServiceMock.Setup(u => u.DeactivateUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var userDto = new UserDto
        {
            Id = userId,
            Email = string.Empty,
            FullName = "Deactivated User"
        };

        _userServiceMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserDto>.Success(userDto));

        var command = new DeactivateUserCommand(userId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();

        _jobSchedulerMock.Verify(j => j.Enqueue(It.IsAny<Expression<Func<IIdentityNotificationService, Task>>>()), Times.Never);
    }
}
