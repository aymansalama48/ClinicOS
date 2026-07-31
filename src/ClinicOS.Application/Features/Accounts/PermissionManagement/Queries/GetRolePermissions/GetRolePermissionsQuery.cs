namespace ClinicOS.Application.Features.Accounts.PermissionManagement.Queries.GetRolePermissions;

using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization.Models;
using ClinicOS.Domain.Constants;

[Permission(Permissions.SettingsManage)]
public sealed record GetRolePermissionsQuery(Guid RoleId) : ICacheableQuery<RoleWithPermissionsDto>
{
    // مفتاح الكاش يتغير بناءً على الـ RoleId لتخزين كل دور على حدة
    public string CacheKey => $"Roles:{RoleId}:Permissions";
}