using ClinicOS.Domain.Enums;
using System;

namespace ClinicOS.Application.Features.Patients.Queries.GetPatientById;

public sealed record PatientDetailsResponse(
    Guid Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName, // الاسم بالكامل
    string PhoneNumber,
    DateOnly? DateOfBirth,
    Gender? Gender,
    BloodType? BloodType,
    string? EmergencyContact,
    Guid? ApplicationUserId,
    bool IsAccountLinked
);