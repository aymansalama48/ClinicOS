using ClinicOS.Application.Common.Abstractions.Identity.Invitations;
using ClinicOS.Application.Features.Accounts.StaffInvitations.Commands.AcceptInvitationWithGoogle;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.StaffInvitations.Commands.AcceptInvitationWithGoogle;

public class AcceptInvitationWithGoogleCommandHandlerTests
{
    private readonly Mock<IInvitationService> _invitationServiceMock;
    private readonly AcceptInvitationWithGoogleCommandHandler _handler;

    public AcceptInvitationWithGoogleCommandHandlerTests()
    {
        _invitationServiceMock = new Mock<IInvitationService>();
        _handler = new AcceptInvitationWithGoogleCommandHandler(_invitationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Google_Invitation_Accepted()
    {
        // Arrange
        _invitationServiceMock.Setup(i => i.AcceptInvitationWithGoogleAsync("token123", "google-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var command = new AcceptInvitationWithGoogleCommand("token123", "google-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Service_Fails()
    {
        // Arrange
        var error = new Error("Invitation.Invalid", "Invalid invitation token", ErrorType.Validation);
        _invitationServiceMock.Setup(i => i.AcceptInvitationWithGoogleAsync("token123", "google-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure(error));

        var command = new AcceptInvitationWithGoogleCommand("token123", "google-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }
}
