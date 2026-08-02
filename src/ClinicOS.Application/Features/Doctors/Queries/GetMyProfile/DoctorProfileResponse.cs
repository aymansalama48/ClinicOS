using System;

namespace ClinicOS.Application.Features.Doctors.Queries.GetMyProfile;

public record DoctorProfileResponse(
    Guid Id,
    Guid SpecializationId,
    string? Bio,
    int? YearsOfExperience,
    decimal ConsultationFee,
    decimal UrgentSurchargeFee,
    bool IsActive
);