using ClinicOS.Application.Common.Abstractions.Identity.Tokens;
using ClinicOS.Application.Features.Accounts.Authentication.Commands.RefreshToken;
using ClinicOS.Application.Features.Accounts.StaffAuth.Shared;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
        _handler = new RefreshTokenCommandHandler(_refreshTokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Token_Is_Refreshed()
    {
        // Arrange
        var response = new StaffAuthResponse
        {
            UserId = Guid.NewGuid(),
            Email = "staff@example.com",
            AccessToken = "new-jwt-access-token",
            RefreshToken = "new-refresh-token",
            ExpiresInSeconds = 3600
        };

        _refreshTokenServiceMock.Setup(r => r.RefreshTokenAsync("old-refresh-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<StaffAuthResponse>.Success(response));

        var command = new RefreshTokenCommand("old-refresh-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("new-jwt-access-token");
        result.Data.RefreshToken.Should().Be("new-refresh-token");
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Refresh_Fails()
    {
        // Arrange
        var error = new Error("Token.Expired", "Refresh token has expired", ErrorType.Validation);
        _refreshTokenServiceMock.Setup(r => r.RefreshTokenAsync("expired-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<StaffAuthResponse>.Failure(error));

        var command = new RefreshTokenCommand("expired-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }
}
