using ClinicOS.Domain.Enums;

namespace ClinicOS.Api.Contracts.PatientAuth;

public sealed record PatientOtpLoginRequest(
    string PhoneNumber,
    string Code,
    OtpPurpose Purpose);
