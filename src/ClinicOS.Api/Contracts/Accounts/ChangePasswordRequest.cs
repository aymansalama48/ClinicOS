namespace ClinicOS.Api.Contracts.Accounts;

public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword);
