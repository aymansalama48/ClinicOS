namespace ClinicOS.Application.Features.Accounts.StaffInvitations.Commands.SendInvitation;

using ClinicOS.Application.Common.Abstractions.Messaging;

// استخدام ICommand<TResponse> المخصصة[cite: 13]
public sealed record SendStaffInvitationCommand(
    string Email,
    string Role,
    Guid? SpecializationId = null) : ICommand<string>;