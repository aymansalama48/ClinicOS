using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Identity.Tokens;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Application.Features.Accounts.Shared;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Infrastructure.Persistence.Data;
using ClinicOS.Infrastructure.Persistence.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicOS.Infrastructure.Identity.Authentication;

/// <summary>
/// تنفيذ خدمة مصادقة الموظفين (Staff: Admin, Doctor, Receptionist)
/// </summary>
public class StaffAuthService : IStaffAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IDateTime _dateTime;
    private readonly AppDbContext _context; // ✅ إضافة DbContext
    private readonly ILogger<StaffAuthService> _logger;

    public StaffAuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenService refreshTokenService,
        IDateTime dateTime,
        AppDbContext context, // ✅ حقن DbContext
        ILogger<StaffAuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenService = refreshTokenService;
        _dateTime = dateTime;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// تسجيل دخول موظف بالداش بورد (Admin / Doctor / Receptionist) عبر البريد وكلمة السر
    /// </summary>
    public async Task<Result<StaffAuthResponse>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        // 1. البحث عن المستخدم
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<StaffAuthResponse>.Failure(UserErrors.InvalidCredentials);

        // 2. التحقق من أن الحساب نشط
        if (!user.IsActive)
            return Result<StaffAuthResponse>.Failure(UserErrors.AccountDeactivated);

        // 3. التحقق من كلمة المرور
        var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, true);
        if (!signInResult.Succeeded)
        {
            if (signInResult.IsLockedOut)
                return Result<StaffAuthResponse>.Failure(UserErrors.AccountLocked);
            if (signInResult.IsNotAllowed)
                return Result<StaffAuthResponse>.Failure(UserErrors.LoginNotAllowed);
            return Result<StaffAuthResponse>.Failure(UserErrors.InvalidCredentials);
        }

        // 4. جلب الأدوار
        var roles = await _userManager.GetRolesAsync(user);

        // 5. جلب الصلاحيات من قاعدة البيانات
        var permissions = await GetUserPermissionsAsync(user.Id, cancellationToken);

        // 6. جلب الـ SpecializationId (إن كان طبيباً أو موظف استقبال)
        var specializationId = await GetSpecializationIdAsync(user.Id, cancellationToken);

        // 7. توليد الـ Access Token
        var accessToken = _jwtTokenGenerator.GenerateStaffToken(
            user.Id,
            user.Email!,
            user.FullName,
            roles,
            permissions,
            specializationId);

        // 8. إنشاء Refresh Token
        var refreshToken = await _refreshTokenService.GenerateAndSaveRefreshTokenAsync(user.Id, cancellationToken);

        // 9. تحديث آخر وقت دخول
        user.LastLoginAt = _dateTime.Now;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("تم تسجيل دخول الموظف {Email} بنجاح", email);

        // 10. إرجاع الـ Response
        return Result<StaffAuthResponse>.Success(new StaffAuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresInSeconds = 3600,
            Roles = roles.ToList(),
            SpecializationId = specializationId
        });
    }

    /// <summary>
    /// تسجيل الخروج وإبطال الـ Refresh Token
    /// </summary>
    public async Task<Result<bool>> LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        // إلغاء الـ Refresh Token المقدم
        await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);

        // تسجيل الخروج من الـ SignIn (جلسة الـ Cookies إن وجدت)
        await _signInManager.SignOutAsync();

        _logger.LogInformation("تم تسجيل خروج الموظف بنجاح (Refresh Token: {Token})", refreshToken);

        return Result<bool>.Success(true);
    }

    // ===== دوال مساعدة =====

    /// <summary>
    /// جلب قائمة الصلاحيات الخاصة بالمستخدم من قاعدة البيانات
    /// </summary>
    private async Task<IList<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return new List<string>();

        // استخدام الـ Claims المخزنة في Identity
        var claims = await _userManager.GetClaimsAsync(user);
        return claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList();
    }

    /// <summary>
    /// جلب معرف التخصص للموظف (إن كان طبيباً أو موظف استقبال)
    /// </summary>
    private async Task<Guid?> GetSpecializationIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        // 1. البحث في جدول الأطباء
        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(d => d.ApplicationUserId == userId && !d.IsDeleted, cancellationToken);

        if (doctor is not null)
            return doctor.SpecializationId;

        // 2. البحث في جدول موظفي الاستقبال
        var receptionist = await _context.Receptionists
            .FirstOrDefaultAsync(r => r.ApplicationUserId == userId && !r.IsDeleted, cancellationToken);

        if (receptionist is not null)
            return receptionist.SpecializationId;

        // 3. إذا كان Admin أو ليس له تخصص، نرجع null
        return null;
    }
}