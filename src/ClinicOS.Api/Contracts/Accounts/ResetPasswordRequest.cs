namespace ClinicOS.Api.Contracts.Accounts;

public sealed record ResetPasswordRequest(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword);
