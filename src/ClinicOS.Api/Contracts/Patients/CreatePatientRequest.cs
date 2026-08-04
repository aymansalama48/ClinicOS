using ClinicOS.Domain.Enums;

namespace ClinicOS.Api.Contracts.Patients;

public sealed record CreatePatientRequest(
    string FirstName,
    string? MiddleName,
    string LastName,
    string PhoneNumber,
    DateOnly? DateOfBirth,
    Gender? Gender,
    BloodType? BloodType,
    string? EmergencyContact,
    Guid? ApplicationUserId);
