using System;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctors;

public sealed record DoctorResponse(
    Guid Id,
    Guid SpecializationId,
    Guid ApplicationUserId,
    string FullName, // 👈 ضفنا الاسم هنا
    string? Email,   // 👈 وممكن الإيميل كمان لو محتاجه
    string? Bio,
    int? YearsOfExperience,
    decimal ConsultationFee,
    decimal UrgentSurchargeFee,
    bool IsActive
);