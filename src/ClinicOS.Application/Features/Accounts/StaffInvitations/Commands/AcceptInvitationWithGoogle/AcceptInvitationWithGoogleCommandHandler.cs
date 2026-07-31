namespace ClinicOS.Application.Features.Accounts.StaffInvitations.Commands.AcceptInvitationWithGoogle;

using ClinicOS.Application.Common.Abstractions.Identity.Invitations;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Common.Results;

// استخدام ICommandHandler<TCommand, TResponse> المخصصة[cite: 14]
public sealed class AcceptInvitationWithGoogleCommandHandler(
    IInvitationService invitationService)
    : ICommandHandler<AcceptInvitationWithGoogleCommand, bool>
{
    public async Task<Result<bool>> Handle(
        AcceptInvitationWithGoogleCommand request,
        CancellationToken cancellationToken)
    {
        return await invitationService.AcceptInvitationWithGoogleAsync(
            request.InvitationToken,
            request.GoogleIdToken,
            cancellationToken);
    }
}