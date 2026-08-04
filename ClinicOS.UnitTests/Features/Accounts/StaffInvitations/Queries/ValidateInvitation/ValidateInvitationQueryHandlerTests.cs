using ClinicOS.Application.Common.Abstractions.Identity.Invitations;
using ClinicOS.Application.Features.Accounts.StaffInvitations.Queries.ValidateInvitation;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.StaffInvitations.Queries.ValidateInvitation;

public class ValidateInvitationQueryHandlerTests
{
    private readonly Mock<IInvitationService> _invitationServiceMock;
    private readonly ValidateInvitationQueryHandler _handler;

    public ValidateInvitationQueryHandlerTests()
    {
        _invitationServiceMock = new Mock<IInvitationService>();
        _handler = new ValidateInvitationQueryHandler(_invitationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Token_Is_Valid()
    {
        // Arrange
        var dto = new InvitationDetailsDto(
            Guid.NewGuid(),
            "doctor@clinic.com",
            "Doctor",
            "Admin User",
            null,
            true);

        _invitationServiceMock.Setup(i => i.ValidateInvitationTokenAsync("valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvitationDetailsDto>.Success(dto));

        var query = new ValidateInvitationQuery("valid-token");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(dto);
        result.Data!.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Token_Is_Invalid()
    {
        // Arrange
        var error = new Error("Invitation.Invalid", "Token is invalid or expired", ErrorType.Validation);
        _invitationServiceMock.Setup(i => i.ValidateInvitationTokenAsync("expired-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvitationDetailsDto>.Failure(error));

        var query = new ValidateInvitationQuery("expired-token");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }
}
