namespace ClinicOS.Api.Contracts.Doctors;

public sealed record CompleteMyDoctorProfileRequest(
    Guid SpecializationId,
    string? Bio,
    int? YearsOfExperience,
    decimal ConsultationFee,
    decimal UrgentSurchargeFee);
