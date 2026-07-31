using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Constants;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ClinicOS.Infrastructure.Identity.CurrentUser;

/// <summary>
/// خدمة جلب بيانات اليوزر الحالي من الـ Claims بتاعة الـ JWT الحالي
/// </summary>
public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly ClaimsPrincipal? _user = httpContextAccessor.HttpContext?.User;

    /// <summary>
    /// هل اليوزر مسجل دخوله أصلاً؟
    /// </summary>
    public bool IsAuthenticated =>
        _user?.Identity?.IsAuthenticated ?? false;

    // === مشتركة بين المريض والـ Staff ===

    /// <summary>
    /// معرف الـ ApplicationUser (موجود في توكن الـ Staff)
    /// </summary>
    public Guid? UserId =>
        ParseGuid(_user?.FindFirstValue(CustomClaims.UserId));

    /// <summary>
    /// الاسم الكامل
    /// </summary>
    public string? FullName =>
        _user?.FindFirstValue(ClaimTypes.Name);

    // === خاصة بالمريض فقط ===

    /// <summary>
    /// معرف المريض (موجود في توكن المريض)
    /// </summary>
    public Guid? PatientId =>
        ParseGuid(_user?.FindFirstValue(CustomClaims.PatientId));

    /// <summary>
    /// رقم الموبايل
    /// </summary>
    public string? PhoneNumber =>
        _user?.FindFirstValue(ClaimTypes.MobilePhone);

    // === خاصة بالـ Staff فقط ===

    /// <summary>
    /// الإيميل
    /// </summary>
    public string? Email =>
        _user?.FindFirstValue(ClaimTypes.Email);

    /// <summary>
    /// معرف التخصص (للدكاترة)
    /// </summary>
    public Guid? SpecializationId =>
        ParseGuid(_user?.FindFirstValue(CustomClaims.SpecializationId));

    /// <summary>
    /// أول دور لليوزر
    /// </summary>
    public string? Role =>
        _user?.FindFirstValue(ClaimTypes.Role);

    /// <summary>
    /// كل الأدوار
    /// </summary>
    public IReadOnlyList<string> Roles =>
        _user?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        ?? new List<string>();

    /// <summary>
    /// هل اليوزر في دور معين؟
    /// </summary>
    public bool IsInRole(string role) =>
        _user?.IsInRole(role) ?? false;

    /// <summary>
    /// هل اليوزر معاه صلاحية معينة؟
    /// </summary>
    public bool HasPermission(string permission) =>
        _user?.FindAll(CustomClaims.Permission).Any(c => c.Value == permission) ?? false;

    /// <summary>
    /// كل صلاحيات اليوزر
    /// </summary>
    public IEnumerable<string> GetPermissions() =>
        _user?.FindAll(CustomClaims.Permission).Select(c => c.Value)
        ?? Enumerable.Empty<string>();

    private static Guid? ParseGuid(string? value) =>
        Guid.TryParse(value, out var guid) ? guid : null;
}