using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Identity.Tokens;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Application.Features.Accounts.Shared;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Security;
using ClinicOS.Domain.Security;
using ClinicOS.Infrastructure.Persistence.Data;
using ClinicOS.Infrastructure.Persistence.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicOS.Infrastructure.Identity.Tokens;

/// <summary>
/// تنفيذ خدمة Refresh Token الخاصة بالموظفين (Staff)
/// </summary>
public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDateTime _dateTime;
    private readonly ILogger<RefreshTokenService> _logger;

    // مدة صلاحية Refresh Token (7 أيام)
    private static readonly TimeSpan RefreshTokenExpiry = TimeSpan.FromDays(7);

    public RefreshTokenService(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IDateTime dateTime,
        ILogger<RefreshTokenService> logger)
    {
        _context = context;
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dateTime = dateTime;
        _logger = logger;
    }

    /// <summary>
    /// إنشاء وتخزين Refresh Token جديد للموظف
    /// </summary>
    public async Task<string> GenerateAndSaveRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        // 1. إلغاء أي Refresh Tokens سابقة لنفس المستخدم (اختياري - نفضل إلغاء القديم)
        await RevokeAllUserTokensAsync(userId, cancellationToken);

        // 2. توليد Token عشوائي آمن
        var token = GenerateSecureToken();

        // 3. إنشاء الكيان
        var refreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Token = token,
            ExpiryDate = _dateTime.Now.Add(RefreshTokenExpiry),
            CreatedAt = _dateTime.Now,
            IsRevoked = false
        };

        // 4. الحفظ في قاعدة البيانات
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("تم إنشاء Refresh Token جديد للمستخدم {UserId}", userId);

        return token;
    }

    /// <summary>
    /// تجديد الـ Access Token باستخدام Refresh Token صالح
    /// </summary>
    public async Task<Result<StaffAuthResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        // 1. البحث عن الـ Refresh Token في قاعدة البيانات
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (storedToken is null)
            return Result<StaffAuthResponse>.Failure(TokenErrors.InvalidRefreshToken);

        // 2. التحقق من صلاحيته
        if (storedToken.IsRevoked)
            return Result<StaffAuthResponse>.Failure(TokenErrors.TokenRevoked);

        if (storedToken.ExpiryDate < _dateTime.Now)
            return Result<StaffAuthResponse>.Failure(TokenErrors.TokenExpired);

        // 3. جلب المستخدم
        var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
        if (user is null)
            return Result<StaffAuthResponse>.Failure(UserErrors.NotFound);

        // 4. جلب الأدوار والصلاحيات
        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await _context.Roles
            .Where(r => roles.Contains(r.Name!))
            .SelectMany(r => r.RolePermissions)
            .Select(rp => rp.Permission!.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        // 5. جلب الـ SpecializationId إن كان المستخدم طبيباً أو موظف استقبال
        // (هذا يعتمد على وجود SpecializationId في الـ Claims أو عن طريق استعلام منفصل)
        // سنفترض أن المستخدم لديه SpecializationId في جدول Doctor أو Receptionist
        // ولكننا سنبقيها اختيارية:
        Guid? specializationId = null;

        // 6. توليد Access Token جديد
        var accessToken = _jwtTokenGenerator.GenerateStaffToken(
            user.Id,
            user.Email!,
            user.FullName,
            roles,
            permissions,
            specializationId);

        // 7. تحديث الـ Refresh Token (تجديد صلاحيته مع الاحتفاظ بنفس القيمة - أو إصدار جديد)
        // الطريقة الأكثر أماناً: إصدار Refresh Token جديد وإلغاء القديم (Rotation)
        // لكن سنتبع نهج التجديد (Renew) مع إلغاء القديم:
        storedToken.IsRevoked = true;
        storedToken.LastUsedAt = _dateTime.Now;

        var newRefreshToken = await GenerateAndSaveRefreshTokenAsync(user.Id, cancellationToken);

        // 8. إرجاع الـ Response
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
    /// إلغاء صلاحية Refresh Token معين
    /// </summary>
    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (storedToken is not null)
        {
            storedToken.IsRevoked = true;
            storedToken.LastUsedAt = _dateTime.Now;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("تم إلغاء Refresh Token: {Token}", refreshToken);
        }
    }

    /// <summary>
    /// إلغاء كافة جلسات المستخدم عبر جميع الأجهزة (Global Revoke)
    /// </summary>
    public async Task<Result> RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        if (tokens.Count == 0)
            return Result.Success("لا توجد جلسات نشطة");

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("تم إلغاء جميع Refresh Tokens للمستخدم {UserId} (عدد: {Count})", userId, tokens.Count);

        return Result.Success($"تم إلغاء {tokens.Count} جلسة");
    }

    // ===== دوال مساعدة =====

    /// <summary>
    /// توليد رمز عشوائي آمن بطول 256 بت (Base64)
    /// </summary>
    private static string GenerateSecureToken()
    {
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var bytes = new byte[32]; // 256 بت
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}