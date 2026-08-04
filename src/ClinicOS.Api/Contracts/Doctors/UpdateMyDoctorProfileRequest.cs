namespace ClinicOS.Api.Contracts.Doctors;

public sealed record UpdateMyDoctorProfileRequest(
    Guid SpecializationId,
    string? Bio,
    int? YearsOfExperience,
    decimal ConsultationFee,
    decimal UrgentSurchargeFee);
