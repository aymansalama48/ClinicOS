using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Features.Accounts.PatientAuth.Commands.PatientEmailLogin;
using ClinicOS.Application.Features.Accounts.PatientAuth.Shared;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.PatientAuth.Commands.PatientEmailLogin;

public class PatientEmailLoginCommandHandlerTests
{
    private readonly Mock<IPatientAuthService> _patientAuthServiceMock;
    private readonly PatientEmailLoginCommandHandler _handler;

    public PatientEmailLoginCommandHandlerTests()
    {
        _patientAuthServiceMock = new Mock<IPatientAuthService>();
        _handler = new PatientEmailLoginCommandHandler(_patientAuthServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Credentials_Are_Valid()
    {
        // Arrange
        var response = new PatientAuthResponse(Guid.NewGuid(), "jwt-token", 3600, true);
        _patientAuthServiceMock.Setup(s => s.LoginWithEmailAsync("patient@example.com", "Password123!", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PatientAuthResponse>.Success(response));

        var command = new PatientEmailLoginCommand("patient@example.com", "Password123!");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(response);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Credentials_Are_Invalid()
    {
        // Arrange
        var error = new Error("Auth.InvalidCredentials", "Invalid email or password", ErrorType.Validation);
        _patientAuthServiceMock.Setup(s => s.LoginWithEmailAsync("patient@example.com", "WrongPassword", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PatientAuthResponse>.Failure(error));

        var command = new PatientEmailLoginCommand("patient@example.com", "WrongPassword");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }
}
