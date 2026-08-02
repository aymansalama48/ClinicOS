namespace ClinicOS.Application.Features.Accounts.AccountManagement.Queries.GetMyProfile;

public record MyAccountProfileResponse(
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    string Email,
    string? PhoneNumber,
    string? AvatarUrl
);