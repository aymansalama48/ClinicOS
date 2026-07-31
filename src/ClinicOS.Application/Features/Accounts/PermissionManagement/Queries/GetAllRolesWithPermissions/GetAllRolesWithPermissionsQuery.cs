namespace ClinicOS.Application.Features.Accounts.PermissionManagement.Queries.GetAllRolesWithPermissions;

using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization.Models;
using ClinicOS.Domain.Constants;

[Permission(Permissions.SettingsManage)]
public sealed record GetAllRolesWithPermissionsQuery() : ICacheableQuery<List<RoleWithPermissionsDto>>
{
    public string CacheKey => "Roles:AllWithPermissions";
}