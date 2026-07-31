namespace ClinicOS.Application.Features.Accounts.StaffInvitations.Commands.AcceptInvitation;

using ClinicOS.Application.Common.Abstractions.Identity.Invitations;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Common.Results;

// استخدام ICommandHandler<TCommand, TResponse> المخصصة[cite: 14]
public sealed class AcceptInvitationCommandHandler(
    IInvitationService invitationService)
    : ICommandHandler<AcceptInvitationCommand, bool>
{
    public async Task<Result<bool>> Handle(
        AcceptInvitationCommand request,
        CancellationToken cancellationToken)
    {
        return await invitationService.AcceptInvitationAndCreateAccountAsync(
            request.InvitationToken,
            request.FullName,
            request.Password,
            request.PhoneNumber,
            cancellationToken);
    }
}