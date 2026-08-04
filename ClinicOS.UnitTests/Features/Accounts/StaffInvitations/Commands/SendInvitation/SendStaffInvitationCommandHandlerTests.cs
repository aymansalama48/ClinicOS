using ClinicOS.Application.Common.Abstractions.Identity.Invitations;
using ClinicOS.Application.Features.Accounts.StaffInvitations.Commands.SendInvitation;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.StaffInvitations.Commands.SendInvitation;

public class SendStaffInvitationCommandHandlerTests
{
    private readonly Mock<IInvitationService> _invitationServiceMock;
    private readonly SendStaffInvitationCommandHandler _handler;

    public SendStaffInvitationCommandHandlerTests()
    {
        _invitationServiceMock = new Mock<IInvitationService>();
        _handler = new SendStaffInvitationCommandHandler(_invitationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Invitation_Is_Sent()
    {
        // Arrange
        var token = "invitation-token-xyz";
        _invitationServiceMock.Setup(i => i.SendStaffInvitationAsync("staff@clinic.com", "Doctor", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success(token));

        var command = new SendStaffInvitationCommand("staff@clinic.com", "Doctor", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(token);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Service_Fails()
    {
        // Arrange
        var error = new Error("Invitation.EmailExists", "User already registered", ErrorType.Validation);
        _invitationServiceMock.Setup(i => i.SendStaffInvitationAsync("staff@clinic.com", "Doctor", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Failure(error));

        var command = new SendStaffInvitationCommand("staff@clinic.com", "Doctor", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }
}
