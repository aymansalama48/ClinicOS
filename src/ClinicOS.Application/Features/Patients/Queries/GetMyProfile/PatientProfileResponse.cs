using ClinicOS.Domain.Enums;
using System;

namespace ClinicOS.Application.Features.Patients.Queries.GetMyProfile;

public record PatientProfileResponse(
    Guid Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    string PhoneNumber,
    DateOnly? DateOfBirth,
    Gender? Gender,
    BloodType? BloodType,
    string? EmergencyContact,
    bool IsAccountLinked
);