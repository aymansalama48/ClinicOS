namespace ClinicOS.Api.Contracts.PatientAuth;

public sealed record PatientEmailLoginRequest(
    string Email,
    string Password);
