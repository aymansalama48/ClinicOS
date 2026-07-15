using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Constants;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ClinicOS.Infrastructure.Identity.CurrentUser;

public class CurrentUserService : ICurrentUser
{
    private readonly ClaimsPrincipal? _user;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _user = httpContextAccessor.HttpContext?.User;
    }

    public bool IsAuthenticated =>
        _user?.Identity?.IsAuthenticated ?? false;

    // === مشتركة بين المريض والـ Staff ===

    public Guid? UserId =>
        ParseGuid(_user?.FindFirstValue(CustomClaims.UserId));

    public string? FullName =>
        _user?.FindFirstValue(ClaimTypes.Name);

    // === خاصة بالمريض فقط ===

    public Guid? PatientId =>
        ParseGuid(_user?.FindFirstValue(CustomClaims.PatientId));

    public string? PhoneNumber =>
        _user?.FindFirstValue(ClaimTypes.MobilePhone);

    // === خاصة بالـ Staff فقط ===

    public string? Email =>
        _user?.FindFirstValue(ClaimTypes.Email);

    public Guid? SpecializationId =>
        ParseGuid(_user?.FindFirstValue(CustomClaims.SpecializationId));

    public string? Role =>
        _user?.FindFirstValue(ClaimTypes.Role);

    public IReadOnlyList<string> Roles =>
        _user?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        ?? new List<string>();

    public bool IsInRole(string role) =>
        _user?.IsInRole(role) ?? false;

    public bool HasPermission(string permission) =>
        _user?.FindAll(CustomClaims.Permission).Any(c => c.Value == permission) ?? false;

    public IEnumerable<string> GetPermissions() =>
        _user?.FindAll(CustomClaims.Permission).Select(c => c.Value)
        ?? Enumerable.Empty<string>();

    private static Guid? ParseGuid(string? value) =>
        Guid.TryParse(value, out var guid) ? guid : null;
}