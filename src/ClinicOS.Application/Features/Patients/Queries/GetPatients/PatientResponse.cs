using ClinicOS.Domain.Enums;
using System;

namespace ClinicOS.Application.Features.Patients.Queries.GetPatients;

public sealed record PatientResponse(
    Guid Id,
    string FullName, // هنجمعه في الـ Select
    string PhoneNumber,
    DateOnly? DateOfBirth,
    Gender? Gender,
    BloodType? BloodType,
    bool IsAccountLinked // هنحسبها بناءً على وجود الـ ApplicationUserId
);