namespace ClinicOS.Api.Contracts.PatientAuth;

public sealed record RegisterPermanentAccountRequest(
    string Email,
    string Password,
    string PhoneNumber,
    string OtpCode);
