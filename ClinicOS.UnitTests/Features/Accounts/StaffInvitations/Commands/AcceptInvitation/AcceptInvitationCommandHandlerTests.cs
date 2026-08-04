using ClinicOS.Application.Common.Abstractions.Identity.Invitations;
using ClinicOS.Application.Features.Accounts.StaffInvitations.Commands.AcceptInvitation;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.StaffInvitations.Commands.AcceptInvitation;

public class AcceptInvitationCommandHandlerTests
{
    private readonly Mock<IInvitationService> _invitationServiceMock;
    private readonly AcceptInvitationCommandHandler _handler;

    public AcceptInvitationCommandHandlerTests()
    {
        _invitationServiceMock = new Mock<IInvitationService>();
        _handler = new AcceptInvitationCommandHandler(_invitationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Invitation_Accepted()
    {
        // Arrange
        _invitationServiceMock.Setup(i => i.AcceptInvitationAndCreateAccountAsync(
            "token123",
            "Dr. Tamer",
            "Password123!",
            "01012345678",
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var command = new AcceptInvitationCommand("token123", "Dr. Tamer", "Password123!", "01012345678");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Invitation_Is_Invalid_Or_Expired()
    {
        // Arrange
        var error = new Error("Invitation.Invalid", "Invitation token invalid or expired", ErrorType.Validation);
        _invitationServiceMock.Setup(i => i.AcceptInvitationAndCreateAccountAsync(
            "invalid-token",
            "Dr. Tamer",
            "Password123!",
            "01012345678",
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure(error));

        var command = new AcceptInvitationCommand("invalid-token", "Dr. Tamer", "Password123!", "01012345678");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }
}
