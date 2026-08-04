using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Features.Accounts.PatientAuth.Commands.PatientGoogleLogin;
using ClinicOS.Application.Features.Accounts.PatientAuth.Shared;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.PatientAuth.Commands.PatientGoogleLogin;

public class PatientGoogleLoginCommandHandlerTests
{
    private readonly Mock<IPatientAuthService> _patientAuthServiceMock;
    private readonly PatientGoogleLoginCommandHandler _handler;

    public PatientGoogleLoginCommandHandlerTests()
    {
        _patientAuthServiceMock = new Mock<IPatientAuthService>();
        _handler = new PatientGoogleLoginCommandHandler(_patientAuthServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Token_Is_Valid()
    {
        // Arrange
        var response = new PatientAuthResponse(Guid.NewGuid(), "jwt-token", 3600, true);
        _patientAuthServiceMock.Setup(s => s.LoginWithGoogleAsync("valid-google-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PatientAuthResponse>.Success(response));

        var command = new PatientGoogleLoginCommand("valid-google-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(response);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Token_Is_Invalid()
    {
        // Arrange
        var error = new Error("Google.InvalidToken", "Invalid Google Token", ErrorType.Validation);
        _patientAuthServiceMock.Setup(s => s.LoginWithGoogleAsync("invalid-google-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PatientAuthResponse>.Failure(error));

        var command = new PatientGoogleLoginCommand("invalid-google-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }
}
