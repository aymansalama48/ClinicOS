using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.External.Routing;
using ClinicOS.Application.Common.Abstractions.Identity.Security;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Application.Common.Abstractions.Web;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ForgotPassword;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.AccountManagement.Commands.ForgotPassword;

public class ForgotPasswordCommandHandlerTests
{
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IApplicationUrlService> _urlServiceMock;
    private readonly Mock<IJobScheduler> _jobSchedulerMock;
    private readonly Mock<IClientContext> _clientContextMock;
    private readonly ForgotPasswordCommandHandler _handler;

    public ForgotPasswordCommandHandlerTests()
    {
        _passwordServiceMock = new Mock<IPasswordService>();
        _urlServiceMock = new Mock<IApplicationUrlService>();
        _jobSchedulerMock = new Mock<IJobScheduler>();
        _clientContextMock = new Mock<IClientContext>();

        _handler = new ForgotPasswordCommandHandler(
            _passwordServiceMock.Object,
            _urlServiceMock.Object,
            _jobSchedulerMock.Object,
            _clientContextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Service_Fails()
    {
        // Arrange
        var error = new Error("Forgot.Failed", "Failed", ErrorType.Failure);
        _passwordServiceMock.Setup(p => p.ForgotPasswordAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Failure(error));

        var command = new ForgotPasswordCommand("user@example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_Without_Email_When_Token_Is_Null_Or_Empty()
    {
        // Arrange
        _passwordServiceMock.Setup(p => p.ForgotPasswordAsync("nonexistent@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success(string.Empty));

        var command = new ForgotPasswordCommand("nonexistent@example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _jobSchedulerMock.Verify(j => j.Enqueue(It.IsAny<Expression<Func<IIdentityNotificationService, Task>>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Generate_Url_And_Enqueue_Email_When_Token_Returned()
    {
        // Arrange
        var token = "sample-reset-token";
        _passwordServiceMock.Setup(p => p.ForgotPasswordAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success(token));

        _urlServiceMock.Setup(u => u.GeneratePasswordResetUrl("user@example.com", token))
            .Returns("https://example.com/reset-password?token=sample-reset-token");

        _clientContextMock.Setup(c => c.IpAddress).Returns("127.0.0.1");
        _clientContextMock.Setup(c => c.UserAgent).Returns("TestAgent");

        var command = new ForgotPasswordCommand("user@example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _jobSchedulerMock.Verify(j => j.Enqueue(It.IsAny<Expression<Func<IIdentityNotificationService, Task>>>()), Times.Once);
    }
}
