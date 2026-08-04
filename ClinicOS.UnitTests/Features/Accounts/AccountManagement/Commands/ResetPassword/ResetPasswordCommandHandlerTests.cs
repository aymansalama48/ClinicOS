using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.Identity.Security;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Application.Common.Abstractions.Web;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ResetPassword;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.AccountManagement.Commands.ResetPassword;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IJobScheduler> _jobSchedulerMock;
    private readonly Mock<IClientContext> _clientContextMock;
    private readonly ResetPasswordCommandHandler _handler;

    public ResetPasswordCommandHandlerTests()
    {
        _passwordServiceMock = new Mock<IPasswordService>();
        _jobSchedulerMock = new Mock<IJobScheduler>();
        _clientContextMock = new Mock<IClientContext>();

        _handler = new ResetPasswordCommandHandler(
            _passwordServiceMock.Object,
            _jobSchedulerMock.Object,
            _clientContextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Reset_Fails()
    {
        // Arrange
        var error = new Error("Reset.InvalidToken", "Token invalid or expired", ErrorType.Validation);
        _passwordServiceMock.Setup(p => p.ResetPasswordAsync("user@example.com", "bad-token", "NewPass123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(error));

        var command = new ResetPasswordCommand("user@example.com", "bad-token", "NewPass123", "NewPass123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);

        _jobSchedulerMock.Verify(j => j.Enqueue(It.IsAny<Expression<Func<IIdentityNotificationService, Task>>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Succeed_And_Enqueue_Notification_When_Valid()
    {
        // Arrange
        _passwordServiceMock.Setup(p => p.ResetPasswordAsync("user@example.com", "valid-token", "NewPass123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _clientContextMock.Setup(c => c.IpAddress).Returns("127.0.0.1");
        _clientContextMock.Setup(c => c.UserAgent).Returns("TestAgent");

        var command = new ResetPasswordCommand("user@example.com", "valid-token", "NewPass123", "NewPass123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _jobSchedulerMock.Verify(j => j.Enqueue(It.IsAny<Expression<Func<IIdentityNotificationService, Task>>>()), Times.Once);
    }
}
