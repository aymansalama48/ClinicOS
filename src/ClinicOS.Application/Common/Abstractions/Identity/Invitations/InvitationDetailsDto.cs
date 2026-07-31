namespace ClinicOS.Application.Common.Abstractions.Identity.Invitations;

public sealed record InvitationDetailsDto(
    Guid InvitationId,
    string Email,
    string Role,
    string AdminName,
    Guid? SpecializationId,
    bool IsValid);