using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.AssignRoleToUser;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.AccountManagement.Commands.AssignRoleToUser;

public class AssignRoleToUserCommandHandlerTests
{
    private readonly Mock<IUserManagementService> _userServiceMock;
    private readonly Mock<IJobScheduler> _jobSchedulerMock;
    private readonly AssignRoleToUserCommandHandler _handler;

    public AssignRoleToUserCommandHandlerTests()
    {
        _userServiceMock = new Mock<IUserManagementService>();
        _jobSchedulerMock = new Mock<IJobScheduler>();

        _handler = new AssignRoleToUserCommandHandler(
            _userServiceMock.Object,
            _jobSchedulerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Role_Assignment_Fails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var error = new Error("Role.AssignFailed", "Failed to assign role", ErrorType.Validation);

        _userServiceMock.Setup(u => u.AssignRoleAsync(userId, "Doctor", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(error));

        var command = new AssignRoleToUserCommand(userId, "Doctor");

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
        _userServiceMock.Setup(u => u.AssignRoleAsync(userId, "Doctor", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var userDto = new UserDto
        {
            Id = userId,
            Email = "doctor@clinic.com",
            FullName = "Dr. Mohamed"
        };

        _userServiceMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserDto>.Success(userDto));

        var command = new AssignRoleToUserCommand(userId, "Doctor");

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
        _userServiceMock.Setup(u => u.AssignRoleAsync(userId, "Doctor", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var userDto = new UserDto
        {
            Id = userId,
            Email = string.Empty,
            FullName = "Dr. Mohamed"
        };

        _userServiceMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserDto>.Success(userDto));

        var command = new AssignRoleToUserCommand(userId, "Doctor");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();

        _jobSchedulerMock.Verify(j => j.Enqueue(It.IsAny<Expression<Func<IIdentityNotificationService, Task>>>()), Times.Never);
    }
}
