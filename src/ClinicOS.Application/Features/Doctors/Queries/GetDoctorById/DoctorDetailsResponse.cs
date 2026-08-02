using System;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctorById;

public sealed record DoctorDetailsResponse(
    Guid Id,
    Guid SpecializationId,
    Guid ApplicationUserId,
    string FullName,   // 👈 تمت الإضافة
    string? Email,     // 👈 تمت الإضافة
    string? AvatarUrl, // 👈 تمت الإضافة
    string? Bio,
    int? YearsOfExperience,
    decimal ConsultationFee,
    decimal UrgentSurchargeFee,
    bool IsActive
);