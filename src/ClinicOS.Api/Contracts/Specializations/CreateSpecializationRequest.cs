namespace ClinicOS.Api.Contracts.Specializations;

public sealed record CreateSpecializationRequest(
    string Name,
    string? Description);
