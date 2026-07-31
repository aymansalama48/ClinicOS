namespace ClinicOS.Application.Features.Accounts.StaffInvitations.Queries.ValidateInvitation;

using ClinicOS.Application.Common.Abstractions.Identity.Invitations;
using ClinicOS.Application.Common.Abstractions.Messaging;

// استخدام IQuery<TResponse> المخصصة[cite: 15]
public sealed record ValidateInvitationQuery(string Token) : IQuery<InvitationDetailsDto>;