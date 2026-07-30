using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Identity.Providers;
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
    private readonly IPermissionService _permissionService;   // 👈 مضافة بدل الـ Claims المباشرة
    private readonly IDateTime _dateTime;
    private readonly ISpecializationService _specializationService;
    private readonly AppDbContext _context;
    private readonly IEnumerable<IExternalAuthProvider> _externalAuthProviders;   // 👈 جديدة في الكونستركتور

    private readonly ILogger<StaffAuthService> _logger;

    public StaffAuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenService refreshTokenService,
        IPermissionService permissionService,
        IDateTime dateTime,
        ISpecializationService specializationService,
        AppDbContext context,
        IEnumerable<IExternalAuthProvider> externalAuthProviders,
        ILogger<StaffAuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenService = refreshTokenService;
        _permissionService = permissionService;
        _dateTime = dateTime;
        _specializationService = specializationService;
        _context = context;
        _externalAuthProviders = externalAuthProviders;
        _logger = logger;
    }

    public async Task<Result<StaffAuthResponse>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<StaffAuthResponse>.Failure(UserErrors.InvalidCredentials);

        if (!user.IsActive)
            return Result<StaffAuthResponse>.Failure(UserErrors.AccountDeactivated);

        // CheckPasswordSignInAsync بدل PasswordSignInAsync — عشان منعملش Cookie Sign-in
        // غير مطلوب في API قايم على JWT بس، مع الاحتفاظ بنفس فايدة الـ Lockout
        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!signInResult.Succeeded)
        {
            if (signInResult.IsLockedOut)
                return Result<StaffAuthResponse>.Failure(UserErrors.AccountLocked);
            if (signInResult.IsNotAllowed)
                return Result<StaffAuthResponse>.Failure(UserErrors.LoginNotAllowed);
            return Result<StaffAuthResponse>.Failure(UserErrors.InvalidCredentials);
        }

        var roles = await _userManager.GetRolesAsync(user);

        // الصلاحيات دلوقتي بتيجي من IPermissionService (Role-based) بدل Claims مباشرة على اليوزر
        var permissions = await _permissionService.GetUserPermissionsAsync(user.Id, cancellationToken);

        var specializationId = await _specializationService.GetSpecializationIdAsync(user.Id, cancellationToken);

        var accessToken = _jwtTokenGenerator.GenerateStaffToken(
            user.Id,
            user.Email!,
            user.FullName,
            roles,
            permissions,
            specializationId);

        var refreshToken = await _refreshTokenService.GenerateAndSaveRefreshTokenAsync(user.Id, cancellationToken);

        user.LastLoginAt = _dateTime.Now;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("تم تسجيل دخول الموظف {Email} بنجاح", email);

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

    public async Task<Result<bool>> LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
        await _signInManager.SignOutAsync();

        _logger.LogInformation("تم تسجيل خروج الموظف بنجاح (Refresh Token: {Token})", refreshToken);

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
        var provider = _externalAuthProviders.FirstOrDefault(p => p.ProviderName == "Google");
        if (provider is null)
            return Result<StaffAuthResponse>.Failure(ExternalAuthErrors.InvalidToken);

        var tokenResult = await provider.ValidateTokenAsync(idToken, cancellationToken);
        if (!tokenResult.IsSuccess)
            return Result<StaffAuthResponse>.Failure(tokenResult.Errors);

        var externalUser = tokenResult.Data!;

        var user = await _userManager.FindByEmailAsync(externalUser.Email);
        if (user is null)
            return Result<StaffAuthResponse>.Failure(UserErrors.InvalidCredentials); // "محتاج دعوة من الأدمن الأول"

        if (!user.IsActive)
            return Result<StaffAuthResponse>.Failure(UserErrors.AccountDeactivated);

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count == 0)
            return Result<StaffAuthResponse>.Failure(UserErrors.InvalidCredentials); // مش حساب Staff فعليًا

        var permissions = await _permissionService.GetUserPermissionsAsync(user.Id, cancellationToken);
        var specializationId = await _specializationService.GetSpecializationIdAsync(user.Id, cancellationToken);

        var accessToken = _jwtTokenGenerator.GenerateStaffToken(
            user.Id, user.Email!, user.FullName, roles, permissions, specializationId);

        var refreshToken = await _refreshTokenService.GenerateAndSaveRefreshTokenAsync(user.Id, cancellationToken);

        user.LastLoginAt = _dateTime.Now;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("تم تسجيل دخول الموظف {Email} بجوجل", externalUser.Email);

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