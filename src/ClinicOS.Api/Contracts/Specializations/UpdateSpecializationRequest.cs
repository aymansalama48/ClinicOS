namespace ClinicOS.Api.Contracts.Specializations;

public sealed record UpdateSpecializationRequest(
    string Name,
    string? Description,
    Guid? IconAttachmentId);
