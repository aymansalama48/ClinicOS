namespace ClinicOS.Api.Contracts.PatientAuth;

public sealed record PatientGoogleLoginRequest(
    string IdToken);
