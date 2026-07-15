using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Infrastructure.Persistence.Data;
using ClinicOS.Infrastructure.Persistence.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Identity.Authorization;

public class PermissionService : IPermissionService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public PermissionService(UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<bool> HasPermissionAsync(
        Guid userId,
        string permissionName,
        CancellationToken cancellationToken)
    {
        var permissions = await GetUserPermissionsAsync(userId, cancellationToken);
        return permissions.Contains(permissionName);
    }

    public async Task<IList<string>> GetUserPermissionsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return new List<string>();

        var roleNames = await _userManager.GetRolesAsync(user);
        if (roleNames.Count == 0)
            return new List<string>();

        var permissions = await _context.Roles   // 👈 دلوقتي بترجع ApplicationRole صح بعد تصحيح AppDbContext
            .Where(r => roleNames.Contains(r.Name!))
            .SelectMany(r => r.RolePermissions)
            .Select(rp => rp.Permission!.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        return permissions;
    }
}