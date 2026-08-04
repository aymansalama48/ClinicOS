namespace ClinicOS.Api.Contracts.StaffInvitations;

public sealed record SendStaffInvitationRequest(
    string Email,
    string Role,
    Guid? SpecializationId = null);
