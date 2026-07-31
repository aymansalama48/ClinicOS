namespace ClinicOS.Application.Features.Accounts.StaffInvitations.Commands.SendInvitation;

using ClinicOS.Application.Common.Abstractions.Identity.Invitations;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Common.Results;

// استخدام ICommandHandler<TCommand, TResponse> المخصصة[cite: 14]
public sealed class SendStaffInvitationCommandHandler(
    IInvitationService invitationService)
    : ICommandHandler<SendStaffInvitationCommand, string>
{
    public async Task<Result<string>> Handle(
        SendStaffInvitationCommand request,
        CancellationToken cancellationToken)
    {
        return await invitationService.SendStaffInvitationAsync(
            request.Email,
            request.Role,
            request.SpecializationId,
            cancellationToken);
    }
}