namespace ClinicOS.Api.Contracts.Accounts;

public sealed record LogoutRequest(
    string RefreshToken);
