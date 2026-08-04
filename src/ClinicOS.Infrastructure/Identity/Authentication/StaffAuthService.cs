using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Identity.Providers;
using ClinicOS.Application.Common.Abstractions.Identity.Tokens;
using ClinicOS.Application.Common.Errors.Identity;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Application.Features.Accounts.StaffAuth.Shared;
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
public class StaffAuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenService refreshTokenService,
    IPermissionService permissionService,   // 👈 مضافة بدل الـ Claims المباشرة
    IDateTime dateTime,
    ISpecializationService specializationService,
    AppDbContext context,
    IEnumerable<IExternalAuthProvider> externalAuthProviders,   // 👈 جديدة في الكونستركتور
    ILogger<StaffAuthService> logger) : IStaffAuthService
{
    /// <summary>
    /// تسجيل دخول الموظف بالإيميل والباسورد مع تطبيق الـ Lockout
    /// </summary>
    public async Task<Result<StaffAuthResponse>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<StaffAuthResponse>.Failure(UserErrors.InvalidCredentials);

        if (!user.IsActive)
            return Result<StaffAuthResponse>.Failure(UserErrors.AccountDeactivated);

        // CheckPasswordSignInAsync بدل PasswordSignInAsync — عشان منعملش Cookie Sign-in
        // غير مطلوب في API قايم على JWT بس، مع الاحتفاظ بنفس فايدة الـ Lockout
        var signInResult = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!signInResult.Succeeded)
        {
            if (signInResult.IsLockedOut)
                return Result<StaffAuthResponse>.Failure(UserErrors.AccountLocked);
            if (signInResult.IsNotAllowed)
                return Result<StaffAuthResponse>.Failure(UserErrors.LoginNotAllowed);
            return Result<StaffAuthResponse>.Failure(UserErrors.InvalidCredentials);
        }

        var roles = await userManager.GetRolesAsync(user);

        // الصلاحيات دلوقتي بتيجي من IPermissionService (Role-based) بدل Claims مباشرة على اليوزر
        var permissions = await permissionService.GetUserPermissionsAsync(user.Id, cancellationToken);

        var specializationId = await specializationService.GetSpecializationIdAsync(user.Id, cancellationToken);

        var accessToken = jwtTokenGenerator.GenerateStaffToken(
            user.Id,
            user.Email!,
            user.FullName,
            roles,
            permissions,
            specializationId);

        var refreshToken = await refreshTokenService.GenerateAndSaveRefreshTokenAsync(user.Id, cancellationToken);

        user.LastLoginAt = dateTime.Now;
        await userManager.UpdateAsync(user);

        logger.LogInformation("تم تسجيل دخول الموظف {Email} بنجاح", email);

        return Result<StaffAuthResponse>.Success(new StaffAuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresInSeconds = 3600,
            LoggedInAt = dateTime.Now,
            Roles = roles.ToList(),
            SpecializationId = specializationId
        });
    }

    /// <summary>
    /// تسجيل خروج الموظف — إلغاء الـ Refresh Token و Sign-out
    /// </summary>
    public async Task<Result<bool>> LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        await refreshTokenService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
        await signInManager.SignOutAsync();

        logger.LogInformation("تم تسجيل خروج الموظف بنجاح (Refresh Token: {Token})", refreshToken);

        return Result<bool>.Success(true);
    }

    /// <summary>
    /// دخول Staff بجوجل — لا ينشئ حساب جديد أبدًا، لازم يكون الحساب موجود بالفعل
    /// (اتعمل من قبل عن طريق نظام الدعوات) وإلا نظام الدعوات بيبقى بلا فايدة
    /// </summary>
    public async Task<Result<StaffAuthResponse>> LoginWithGoogleAsync(
            string idToken,
            CancellationToken cancellationToken = default)
    {
        // 1. التأكد من وجود مزود خدمة جوجل
        var provider = externalAuthProviders.FirstOrDefault(p => p.ProviderName == "Google");
        if (provider is null)
            return Result<StaffAuthResponse>.Failure(ExternalAuthErrors.InvalidToken);

        // 2. التحقق من صحة توكن جوجل المرسل
        var tokenResult = await provider.ValidateTokenAsync(idToken, cancellationToken);
        if (!tokenResult.IsSuccess)
            return Result<StaffAuthResponse>.Failure(tokenResult.Errors);

        var externalUser = tokenResult.Data!;

        // 3. البحث عن المستخدم في قاعدة البيانات
        var user = await userManager.FindByEmailAsync(externalUser.Email);

        // ==========================================
        // 4. 👇 التعديل الجديد: استثناء حسابات الإدارة (Auto-Provisioning)
        // ==========================================
        if (user is null)
        {
            // قائمة بإيميلات الإدارة المسموح بإنشاء حسابات تلقائية لها
            var superAdminEmails = new[] { "aymansalama48@yahoo.com", "ayman.dev@hotmail.com" };

            // تجاهل حالة الأحرف عند المقارنة (OrdinalIgnoreCase)
            if (superAdminEmails.Contains(externalUser.Email, StringComparer.OrdinalIgnoreCase))
            {
                // إنشاء مستخدم جديد كـ أدمن
                user = new ApplicationUser
                {
                    UserName = externalUser.Email,
                    Email = externalUser.Email,
                     FirstName = "System",
                     LastName = "Admin",
                    EmailConfirmed = true, // تفعيل الإيميل تلقائياً
                    IsActive = true        // تنشيط الحساب
                };

                // إنشاء المستخدم في الداتا بيز باستخدام UserManager عشان يعمل Hashing و Normalized Email
                var createResult = await userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    logger.LogError("فشل في إنشاء حساب الأدمن التلقائي: {Errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    return Result<StaffAuthResponse>.Failure(UserErrors.InvalidCredentials);
                }

                // إعطاء المستخدم دور الأدمن (يفضل استخدام الثابت الخاص بك بدلاً من النص الصريح مثل Roles.Admin)
                await userManager.AddToRoleAsync(user, "Admin");

                logger.LogInformation("تم إنشاء حساب أدمن تلقائي للإيميل: {Email}", externalUser.Email);
            }
            else
            {
                // أي إيميل آخر غير مسجل في النظام وغير موجود في قائمة الـ Super Admins سيتم رفضه
                return Result<StaffAuthResponse>.Failure(UserErrors.InvalidCredentials);
            }
        }

        // 5. التحقق من أن الحساب نشط وغير موقوف
        if (!user.IsActive)
            return Result<StaffAuthResponse>.Failure(UserErrors.AccountDeactivated);

        // 6. جلب الأدوار (Roles) والصلاحيات (Permissions)
        var roles = await userManager.GetRolesAsync(user);
        if (roles.Count == 0)
            return Result<StaffAuthResponse>.Failure(UserErrors.InvalidCredentials); // مش حساب Staff فعليًا

        var permissions = await permissionService.GetUserPermissionsAsync(user.Id, cancellationToken);
        var specializationId = await specializationService.GetSpecializationIdAsync(user.Id, cancellationToken);

        // 7. توليد الـ Access Token والـ Refresh Token الخاصين بالنظام (ClinicOS)
        var accessToken = jwtTokenGenerator.GenerateStaffToken(
            user.Id, user.Email!, user.FullName, roles, permissions, specializationId);

        var refreshToken = await refreshTokenService.GenerateAndSaveRefreshTokenAsync(user.Id, cancellationToken);

        // 8. تحديث وقت آخر ظهور (Last Login) للمستخدم
        user.LastLoginAt = dateTime.Now;
        await userManager.UpdateAsync(user);

        logger.LogInformation("تم تسجيل دخول الموظف {Email} بجوجل", externalUser.Email);

        // 9. إرجاع النتيجة بنجاح للـ Frontend
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
}