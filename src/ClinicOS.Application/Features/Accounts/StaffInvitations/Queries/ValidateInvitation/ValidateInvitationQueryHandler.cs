namespace ClinicOS.Application.Features.Accounts.StaffInvitations.Queries.ValidateInvitation;

using ClinicOS.Application.Common.Abstractions.Identity.Invitations;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Common.Results;

// استخدام IQueryHandler<TQuery, TResponse> المخصصة[cite: 16]
public sealed class ValidateInvitationQueryHandler(
    IInvitationService invitationService)
    : IQueryHandler<ValidateInvitationQuery, InvitationDetailsDto>
{
    public async Task<Result<InvitationDetailsDto>> Handle(
        ValidateInvitationQuery request,
        CancellationToken cancellationToken)
    {
        return await invitationService.ValidateInvitationTokenAsync(request.Token, cancellationToken);
    }
}