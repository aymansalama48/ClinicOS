using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.Identity.Tokens;
using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Infrastructure.Persistence.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicOS.Infrastructure.Identity.UserManagement;

/// <summary>
/// تنفيذ خدمة إدارة المستخدمين (تخدم Staff والمريض صاحب الحساب الدائم)
/// </summary>
public class UserManagementService(
    UserManager<ApplicationUser> userManager,
    IRefreshTokenService refreshTokenService,
    IDateTime dateTime,
    ILogger<UserManagementService> logger) : IUserManagementService
{
    /// <summary>
    /// جلب بيانات المستخدم بواسطة المعرف
    /// </summary>
    public async Task<Result<UserDto>> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<UserDto>.Failure(UserErrors.NotFound);

        var roles = await userManager.GetRolesAsync(user);

        return Result<UserDto>.Success(new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            Roles = roles.ToList(),
            IsActive = user.IsActive
        });
    }

    /// <summary>
    /// التأكد من وجود المستخدم في النظام
    /// </summary>
    public async Task<Result> EnsureUserExistsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        return Result.Success();
    }

    /// <summary>
    /// تعطيل تسجيل الدخول + إلغاء كل الجلسات الشغالة (Refresh Tokens) فورًا
    /// عشان موظف اتعمله Deactivate ميقدرش يستخدم السيستم حتى لو معاه Refresh Token صالح
    /// </summary>
    public async Task<Result> DeactivateUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        if (!user.IsActive)
            return Result.Success("الحساب معطل بالفعل");

        user.IsActive = false;
        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogWarning("فشل تعطيل المستخدم {UserId}: {Errors}", userId, errors);
            return Result.Failure(UserErrors.UpdateFailed(errors));
        }

        // إلغاء كل الجلسات الشغالة فورًا (كل الأجهزة)
        await refreshTokenService.RevokeAllUserTokensAsync(userId, cancellationToken);

        logger.LogInformation("تم تعطيل المستخدم {UserId} وإلغاء كل جلساته بنجاح", userId);
        return Result.Success("تم تعطيل الحساب وإنهاء كل الجلسات بنجاح");
    }

    /// <summary>
    /// إعادة تفعيل الحساب
    /// </summary>
    public async Task<Result> ActivateUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        if (user.IsActive)
            return Result.Success("الحساب مفعل بالفعل");

        user.IsActive = true;
        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogWarning("فشل تفعيل المستخدم {UserId}: {Errors}", userId, errors);
            return Result.Failure(UserErrors.UpdateFailed(errors));
        }

        logger.LogInformation("تم تفعيل المستخدم {UserId} بنجاح", userId);
        return Result.Success("تم تفعيل الحساب بنجاح");
    }

    /// <summary>
    /// تحديث البيانات الأساسية (الاسم ورقم الهاتف)
    /// </summary>
    public async Task<Result> UpdateProfileAsync(
        Guid userId,
        string fullName,
        string phoneNumber,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        // التحقق من صحة البيانات
        if (string.IsNullOrWhiteSpace(fullName))
            return Result.Failure(UserErrors.ValidationFailed("الاسم مطلوب"));

        // تحديث الاسم (تقسيم الاسم الكامل إلى FirstName و LastName)
        var nameParts = fullName.Trim().Split(' ', 2);
        user.FirstName = nameParts[0];
        user.LastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

        // تحديث رقم الهاتف
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            // التحقق من أن رقم الهاتف غير مستخدم من قبل مستخدم آخر
            var existingUser = await userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber && u.Id != userId, cancellationToken);

            if (existingUser is not null)
                return Result.Failure(UserErrors.PhoneAlreadyExists);

            user.PhoneNumber = phoneNumber;
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogWarning("فشل تحديث الملف الشخصي للمستخدم {UserId}: {Errors}", userId, errors);
            return Result.Failure(UserErrors.UpdateFailed(errors));
        }

        logger.LogInformation("تم تحديث الملف الشخصي للمستخدم {UserId} بنجاح", userId);
        return Result.Success("تم تحديث البيانات بنجاح");
    }

    /// <summary>
    /// تحديث رابط الصورة الشخصية
    /// </summary>
    public async Task<Result> UpdateProfilePictureAsync(
        Guid userId,
        string avatarUrl,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        // التحقق من صحة الرابط (اختياري)
        if (!string.IsNullOrWhiteSpace(avatarUrl) && !Uri.IsWellFormedUriString(avatarUrl, UriKind.Absolute))
            return Result.Failure(UserErrors.ValidationFailed("رابط الصورة غير صالح"));

        user.AvatarUrl = avatarUrl;
        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogWarning("فشل تحديث صورة المستخدم {UserId}: {Errors}", userId, errors);
            return Result.Failure(UserErrors.UpdateFailed(errors));
        }

        logger.LogInformation("تم تحديث صورة المستخدم {UserId} بنجاح", userId);
        return Result.Success("تم تحديث الصورة بنجاح");
    }
}