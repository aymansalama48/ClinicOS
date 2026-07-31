using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
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

namespace ClinicOS.Infrastructure.Identity.Tokens;

/// <summary>
/// تنفيذ خدمة Refresh Token الخاصة بالموظفين (Staff)
/// </summary>
public class RefreshTokenService(
    AppDbContext context,
    UserManager<ApplicationUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator,
    IPermissionService permissionService,   // 👈 مضافة
    ISpecializationService specializationService,
    IDateTime dateTime,
    ILogger<RefreshTokenService> logger) : IRefreshTokenService
{
    private static readonly TimeSpan RefreshTokenExpiry = TimeSpan.FromDays(7);

    /// <summary>
    /// توليد Refresh Token آمن وحفظه في قاعدة البيانات
    /// </summary>
    public async Task<string> GenerateAndSaveRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        // await RevokeAllUserTokensAsync(userId, cancellationToken);

        var token = GenerateSecureToken();

        var refreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Token = token,
            ExpiryDate = dateTime.Now.Add(RefreshTokenExpiry),
            CreatedAt = dateTime.Now,
            IsRevoked = false
        };

        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("تم إنشاء Refresh Token جديد للمستخدم {UserId}", userId);

        return token;
    }

    /// <summary>
    /// تبديل الـ Refresh Token — بيلغي القديم ويرجع Access Token + Refresh Token جديدين
    /// </summary>
    public async Task<Result<StaffAuthResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var storedToken = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (storedToken is null)
            return Result<StaffAuthResponse>.Failure(TokenErrors.InvalidRefreshToken);

        if (storedToken.IsRevoked)
            return Result<StaffAuthResponse>.Failure(TokenErrors.TokenRevoked);

        if (storedToken.ExpiryDate < dateTime.Now)
            return Result<StaffAuthResponse>.Failure(TokenErrors.TokenExpired);

        var user = await userManager.FindByIdAsync(storedToken.UserId.ToString());
        if (user is null)
            return Result<StaffAuthResponse>.Failure(UserErrors.NotFound);

        var roles = await userManager.GetRolesAsync(user);

        // نفس مصدر الصلاحيات المستخدم في LoginAsync بالظبط
        var permissions = await permissionService.GetUserPermissionsAsync(user.Id, cancellationToken);

        // اتصلحت: كانت مثبتة null، دلوقتي بتتجاب فعليًا زي الـ Login بالظبط
        var specializationId = await specializationService.GetSpecializationIdAsync(user.Id, cancellationToken);

        var accessToken = jwtTokenGenerator.GenerateStaffToken(
            user.Id,
            user.Email!,
            user.FullName,
            roles,
            permissions,
            specializationId);

        storedToken.IsRevoked = true;
        storedToken.LastUsedAt = dateTime.Now;

        var newRefreshToken = await GenerateAndSaveRefreshTokenAsync(user.Id, cancellationToken);

        return Result<StaffAuthResponse>.Success(new StaffAuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresInSeconds = 3600,
            Roles = roles.ToList(),
            SpecializationId = specializationId
        });
    }

    /// <summary>
    /// إلغاء Refresh Token معين
    /// </summary>
    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var storedToken = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (storedToken is not null)
        {
            storedToken.IsRevoked = true;
            storedToken.LastUsedAt = dateTime.Now;
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("تم إلغاء Refresh Token: {Token}", refreshToken);
        }
    }

    /// <summary>
    /// إلغاء كل جلسات اليوزر (كل الأجهزة) — بتتستخدم عند تعطيل الحساب مثلاً
    /// </summary>
    public async Task<Result> RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var tokens = await context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        if (tokens.Count == 0)
            return Result.Success("لا توجد جلسات نشطة");

        foreach (var token in tokens)
            token.IsRevoked = true;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("تم إلغاء جميع Refresh Tokens للمستخدم {UserId} (عدد: {Count})", userId, tokens.Count);

        return Result.Success($"تم إلغاء {tokens.Count} جلسة");
    }

    // توليد توكن عشوائي آمن كريبتوجرافيًا
    private static string GenerateSecureToken()
    {
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}