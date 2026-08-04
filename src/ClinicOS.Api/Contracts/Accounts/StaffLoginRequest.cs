namespace ClinicOS.Api.Contracts.Accounts;

public sealed record StaffLoginRequest(
    string Email,
    string Password);
