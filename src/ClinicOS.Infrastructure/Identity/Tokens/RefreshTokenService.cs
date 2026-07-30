using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Identity.Tokens;
using ClinicOS.Application.Common.Errors.Identity;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Application.Features.Accounts.Shared;
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
public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPermissionService _permissionService;   // 👈 مضافة
    private readonly IDateTime _dateTime;
    private readonly ISpecializationService _specializationService;
    private readonly ILogger<RefreshTokenService> _logger;

    private static readonly TimeSpan RefreshTokenExpiry = TimeSpan.FromDays(7);

    public RefreshTokenService(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IPermissionService permissionService,
        ISpecializationService specializationService,
        IDateTime dateTime,
        ILogger<RefreshTokenService> logger)
    {
        _context = context;
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _permissionService = permissionService;
        _specializationService = specializationService;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task<string> GenerateAndSaveRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
       // await RevokeAllUserTokensAsync(userId, cancellationToken);

        var token = GenerateSecureToken();

        var refreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Token = token,
            ExpiryDate = _dateTime.Now.Add(RefreshTokenExpiry),
            CreatedAt = _dateTime.Now,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("تم إنشاء Refresh Token جديد للمستخدم {UserId}", userId);

        return token;
    }

    public async Task<Result<StaffAuthResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (storedToken is null)
            return Result<StaffAuthResponse>.Failure(TokenErrors.InvalidRefreshToken);

        if (storedToken.IsRevoked)
            return Result<StaffAuthResponse>.Failure(TokenErrors.TokenRevoked);

        if (storedToken.ExpiryDate < _dateTime.Now)
            return Result<StaffAuthResponse>.Failure(TokenErrors.TokenExpired);

        var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
        if (user is null)
            return Result<StaffAuthResponse>.Failure(UserErrors.NotFound);

        var roles = await _userManager.GetRolesAsync(user);

        // نفس مصدر الصلاحيات المستخدم في LoginAsync بالظبط
        var permissions = await _permissionService.GetUserPermissionsAsync(user.Id, cancellationToken);

        // اتصلحت: كانت مثبتة null، دلوقتي بتتجاب فعليًا زي الـ Login بالظبط
        var specializationId = await _specializationService.GetSpecializationIdAsync(user.Id, cancellationToken);

        var accessToken = _jwtTokenGenerator.GenerateStaffToken(
            user.Id,
            user.Email!,
            user.FullName,
            roles,
            permissions,
            specializationId);

        storedToken.IsRevoked = true;
        storedToken.LastUsedAt = _dateTime.Now;

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

    public async Task<Result> RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        if (tokens.Count == 0)
            return Result.Success("لا توجد جلسات نشطة");

        foreach (var token in tokens)
            token.IsRevoked = true;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("تم إلغاء جميع Refresh Tokens للمستخدم {UserId} (عدد: {Count})", userId, tokens.Count);

        return Result.Success($"تم إلغاء {tokens.Count} جلسة");
    }

    private static string GenerateSecureToken()
    {
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}