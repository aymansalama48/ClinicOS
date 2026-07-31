namespace ClinicOS.Application.Features.Accounts.StaffInvitations.Commands.AcceptInvitation;

using ClinicOS.Application.Common.Abstractions.Messaging;

public sealed record AcceptInvitationCommand(
    string InvitationToken,
    string FullName,
    string Password,
    string? PhoneNumber = null) : ICommand<bool>;