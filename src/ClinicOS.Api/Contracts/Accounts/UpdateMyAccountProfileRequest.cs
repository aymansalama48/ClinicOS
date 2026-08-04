namespace ClinicOS.Api.Contracts.Accounts;

public sealed record UpdateMyAccountProfileRequest(
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber);
