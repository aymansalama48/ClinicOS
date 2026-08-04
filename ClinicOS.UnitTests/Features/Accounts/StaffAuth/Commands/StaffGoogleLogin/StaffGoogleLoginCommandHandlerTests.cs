using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Application.Common.Abstractions.Web;
using ClinicOS.Application.Features.Accounts.StaffAuth.Commands.StaffGoogleLogin;
using ClinicOS.Application.Features.Accounts.StaffAuth.Shared;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.StaffAuth.Commands.StaffGoogleLogin;

public class StaffGoogleLoginCommandHandlerTests
{
    private readonly Mock<IStaffAuthService> _staffAuthServiceMock;
    private readonly Mock<IJobScheduler> _jobSchedulerMock;
    private readonly Mock<IClientContext> _clientContextMock;
    private readonly StaffGoogleLoginCommandHandler _handler;

    public StaffGoogleLoginCommandHandlerTests()
    {
        _staffAuthServiceMock = new Mock<IStaffAuthService>();
        _jobSchedulerMock = new Mock<IJobScheduler>();
        _clientContextMock = new Mock<IClientContext>();

        _handler = new StaffGoogleLoginCommandHandler(
            _staffAuthServiceMock.Object,
            _jobSchedulerMock.Object,
            _clientContextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Google_Login_Fails()
    {
        // Arrange
        var error = new Error("Google.Failed", "Invalid Google credentials", ErrorType.Validation);
        _staffAuthServiceMock.Setup(s => s.LoginWithGoogleAsync("invalid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<StaffAuthResponse>.Failure(error));

        var command = new StaffGoogleLoginCommand("invalid-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);

        _jobSchedulerMock.Verify(j => j.Enqueue(It.IsAny<Expression<Func<IIdentityNotificationService, Task>>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Succeed_And_Enqueue_LoginEmail_When_Valid()
    {
        // Arrange
        var response = new StaffAuthResponse
        {
            UserId = Guid.NewGuid(),
            FullName = "Dr. Mohamed",
            Email = "mohamed@clinic.com",
            LoggedInAt = DateTime.UtcNow
        };

        _staffAuthServiceMock.Setup(s => s.LoginWithGoogleAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<StaffAuthResponse>.Success(response));

        _clientContextMock.Setup(c => c.IpAddress).Returns("192.168.1.1");
        _clientContextMock.Setup(c => c.UserAgent).Returns("Chrome/100");

        var command = new StaffGoogleLoginCommand("valid-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(response);

        _jobSchedulerMock.Verify(j => j.Enqueue(It.IsAny<Expression<Func<IIdentityNotificationService, Task>>>()), Times.Once);
    }
}
