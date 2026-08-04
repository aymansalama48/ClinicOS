using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Features.Accounts.Authentication.Commands.Logout;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.Authentication.Commands.Logout;

public class LogoutCommandHandlerTests
{
    private readonly Mock<IStaffAuthService> _staffAuthServiceMock;
    private readonly LogoutCommandHandler _handler;

    public LogoutCommandHandlerTests()
    {
        _staffAuthServiceMock = new Mock<IStaffAuthService>();
        _handler = new LogoutCommandHandler(_staffAuthServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Logout_Succeeds()
    {
        // Arrange
        _staffAuthServiceMock.Setup(s => s.LogoutAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var command = new LogoutCommand("valid-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Logout_Fails()
    {
        // Arrange
        var error = new Error("Token.Invalid", "Invalid refresh token", ErrorType.Validation);
        _staffAuthServiceMock.Setup(s => s.LogoutAsync("invalid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure(error));

        var command = new LogoutCommand("invalid-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }
}
