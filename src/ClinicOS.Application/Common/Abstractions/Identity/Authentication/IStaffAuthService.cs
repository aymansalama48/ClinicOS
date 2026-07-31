using ClinicOS.Application.Features.Accounts.StaffAuth.Shared;
using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Common.Abstractions.Identity.Authentication;

public interface IStaffAuthService
{
    /// <summary>
    /// تسجيل دخول موظف بالداش بورد (Admin / Doctor / Receptionist) عبر البريد وكلمة السر
    /// </summary>
    Task<Result<StaffAuthResponse>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// تسجيل الخروج وإبطال الـ Refresh Token
    /// </summary>
    Task<Result<bool>> LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    // IStaffAuthService.cs — إضافة Method جديدة
    Task<Result<StaffAuthResponse>> LoginWithGoogleAsync(
        string idToken,
        CancellationToken cancellationToken = default);
}

//public record StaffAuthResponse(
//    Guid UserId,
//    string Email,
//    string AccessToken,
//    string RefreshToken,
//    int ExpiresInSeconds
//);