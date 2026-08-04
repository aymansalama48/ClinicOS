using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Features.Otps.Commands.RequestOtp;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Enums;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Otps.Commands.RequestOtp;

public class RequestOtpCommandHandlerTests
{
    private readonly Mock<IOtpService> _otpServiceMock;
    private readonly Mock<IJobScheduler> _jobSchedulerMock;
    private readonly RequestOtpCommandHandler _handler;

    public RequestOtpCommandHandlerTests()
    {
        _otpServiceMock = new Mock<IOtpService>();
        _jobSchedulerMock = new Mock<IJobScheduler>();
        _handler = new RequestOtpCommandHandler(_otpServiceMock.Object, _jobSchedulerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_OtpGeneration_Fails()
    {
        // Arrange
        var error = new Error("Otp.Failed", "Failed to generate OTP", ErrorType.Failure);
        _otpServiceMock.Setup(s => s.GenerateOtpAsync(
            It.IsAny<string>(),
            It.IsAny<OtpPurpose>(),
            It.IsAny<Guid?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<OtpGenerationResult>.Failure(error));

        var command = new RequestOtpCommand("01012345678", OtpPurpose.VerifyPhone, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_OtpGeneration_Succeeds()
    {
        // Arrange
        var expiry = DateTime.UtcNow.AddMinutes(5);
        var resend = DateTime.UtcNow.AddMinutes(1);
        var otpData = new OtpGenerationResult
        {
            Code = "123456",
            ExpiresAtUtc = expiry,
            NextResendAllowedAtUtc = resend,
            OtpId = Guid.NewGuid()
        };

        _otpServiceMock.Setup(s => s.GenerateOtpAsync(
            "01012345678",
            OtpPurpose.VerifyPhone,
            null,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<OtpGenerationResult>.Success(otpData));

        var command = new RequestOtpCommand("01012345678", OtpPurpose.VerifyPhone, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ExpiresAtUtc.Should().Be(expiry);
        result.Data.NextResendAllowedAtUtc.Should().Be(resend);
    }
}
