using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Features.Accounts.PatientAuth.Commands.RegisterPermanentAccount;
using ClinicOS.Application.Features.Accounts.PatientAuth.Shared;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.PatientAuth.Commands.RegisterPermanentAccount;

public class RegisterPermanentAccountCommandHandlerTests
{
    private readonly Mock<IPatientAuthService> _patientAuthServiceMock;
    private readonly RegisterPermanentAccountCommandHandler _handler;

    public RegisterPermanentAccountCommandHandlerTests()
    {
        _patientAuthServiceMock = new Mock<IPatientAuthService>();
        _handler = new RegisterPermanentAccountCommandHandler(_patientAuthServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Registration_Succeeds()
    {
        // Arrange
        var response = new PatientAuthResponse(Guid.NewGuid(), "jwt-token", 3600, true);
        _patientAuthServiceMock.Setup(s => s.RegisterPermanentAccountAsync(
            "patient@example.com",
            "Password123!",
            "01012345678",
            "123456",
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PatientAuthResponse>.Success(response));

        var command = new RegisterPermanentAccountCommand(
            "patient@example.com",
            "Password123!",
            "01012345678",
            "123456");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(response);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Registration_Fails()
    {
        // Arrange
        var error = new Error("Account.DuplicateEmail", "Email already registered", ErrorType.Validation);
        _patientAuthServiceMock.Setup(s => s.RegisterPermanentAccountAsync(
            "patient@example.com",
            "Password123!",
            "01012345678",
            "123456",
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PatientAuthResponse>.Failure(error));

        var command = new RegisterPermanentAccountCommand(
            "patient@example.com",
            "Password123!",
            "01012345678",
            "123456");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }
}
