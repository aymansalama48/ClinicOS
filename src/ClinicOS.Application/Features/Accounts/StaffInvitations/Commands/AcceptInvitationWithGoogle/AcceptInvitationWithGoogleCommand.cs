namespace ClinicOS.Application.Features.Accounts.StaffInvitations.Commands.AcceptInvitationWithGoogle;

using ClinicOS.Application.Common.Abstractions.Messaging;

// استخدام ICommand<TResponse> المخصصة[cite: 13]
public sealed record AcceptInvitationWithGoogleCommand(
    string InvitationToken,
    string GoogleIdToken) : ICommand<bool>;