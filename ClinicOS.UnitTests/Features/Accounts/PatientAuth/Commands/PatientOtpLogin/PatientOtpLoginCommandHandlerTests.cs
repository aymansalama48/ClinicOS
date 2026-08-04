using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Features.Accounts.PatientAuth.Commands.PatientOtpLogin;
using ClinicOS.Application.Features.Accounts.PatientAuth.Shared;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Enums;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.PatientAuth.Commands.PatientOtpLogin;

public class PatientOtpLoginCommandHandlerTests
{
    private readonly Mock<IPatientAuthService> _patientAuthServiceMock;
    private readonly PatientOtpLoginCommandHandler _handler;

    public PatientOtpLoginCommandHandlerTests()
    {
        _patientAuthServiceMock = new Mock<IPatientAuthService>();
        _handler = new PatientOtpLoginCommandHandler(_patientAuthServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Otp_Is_Valid()
    {
        // Arrange
        var response = new PatientAuthResponse(Guid.NewGuid(), "jwt-token", 3600, false);
        _patientAuthServiceMock.Setup(s => s.LoginWithOtpAsync("01012345678", "123456", OtpPurpose.AppointmentBooking, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PatientAuthResponse>.Success(response));

        var command = new PatientOtpLoginCommand("01012345678", "123456", OtpPurpose.AppointmentBooking);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(response);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Otp_Is_Invalid()
    {
        // Arrange
        var error = new Error("Otp.Invalid", "Invalid or expired OTP", ErrorType.Validation);
        _patientAuthServiceMock.Setup(s => s.LoginWithOtpAsync("01012345678", "000000", OtpPurpose.AppointmentBooking, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PatientAuthResponse>.Failure(error));

        var command = new PatientOtpLoginCommand("01012345678", "000000", OtpPurpose.AppointmentBooking);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }
}
