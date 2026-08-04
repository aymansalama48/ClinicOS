using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.IdentityTemplats;
using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Identity.Security;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Application.Common.Abstractions.Web;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ChangePassword;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.AccountManagement.Commands.ChangePassword;

public class ChangePasswordCommandHandlerTests
{
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<IJobScheduler> _jobSchedulerMock;
    private readonly Mock<IClientContext> _clientContextMock;
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordCommandHandlerTests()
    {
        _passwordServiceMock = new Mock<IPasswordService>();
        _currentUserMock = new Mock<ICurrentUser>();
        _jobSchedulerMock = new Mock<IJobScheduler>();
        _clientContextMock = new Mock<IClientContext>();

        _handler = new ChangePasswordCommandHandler(
            _passwordServiceMock.Object,
            _currentUserMock.Object,
            _jobSchedulerMock.Object,
            _clientContextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_CurrentUser_Is_Invalid()
    {
        // Arrange
        _currentUserMock.Setup(u => u.UserId).Returns((Guid?)null);
        _currentUserMock.Setup(u => u.Email).Returns((string?)null);

        var command = new ChangePasswordCommand("OldPass123", "NewPass123", "NewPass123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(UserErrors.NotFound);

        _passwordServiceMock.Verify(p => p.ChangePasswordAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_PasswordService_Fails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(u => u.UserId).Returns(userId);
        _currentUserMock.Setup(u => u.Email).Returns("user@example.com");

        var error = new Error("Password.Invalid", "Invalid current password", ErrorType.Validation);
        _passwordServiceMock.Setup(p => p.ChangePasswordAsync(userId, "WrongPass", "NewPass123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(error));

        var command = new ChangePasswordCommand("WrongPass", "NewPass123", "NewPass123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }

    [Fact]
    public async Task Handle_Should_Succeed_And_Enqueue_Notification_When_Valid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(u => u.UserId).Returns(userId);
        _currentUserMock.Setup(u => u.Email).Returns("user@example.com");
        _currentUserMock.Setup(u => u.FullName).Returns("Ahmed Ali");

        _clientContextMock.Setup(c => c.IpAddress).Returns("127.0.0.1");
        _clientContextMock.Setup(c => c.UserAgent).Returns("Mozilla/5.0");

        _passwordServiceMock.Setup(p => p.ChangePasswordAsync(userId, "OldPass123", "NewPass123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var command = new ChangePasswordCommand("OldPass123", "NewPass123", "NewPass123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _jobSchedulerMock.Verify(j => j.Enqueue(It.IsAny<Expression<Func<IIdentityNotificationService, Task>>>()), Times.Once);
    }
}
