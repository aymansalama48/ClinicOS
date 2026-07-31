namespace ClinicOS.Application.Features.Accounts.PermissionManagement.Queries.GetAllPermissions;

using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization.Models;
using ClinicOS.Domain.Constants;

[Permission(Permissions.SettingsManage)]
public sealed record GetAllPermissionsQuery() : ICacheableQuery<List<PermissionDto>>
{
    // مفتاح الكاش ثابت لأن هذه القائمة للجميع
    public string CacheKey => "Permissions:All";
}