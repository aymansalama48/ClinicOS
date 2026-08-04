namespace ClinicOS.Application.Features.Accounts.PermissionManagement.Queries.GetAllRolesWithPermissions;

using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization.Models;
using ClinicOS.Domain.Constants;
using System;

[Permission(Permissions.Users.View)]
public sealed record GetAllRolesWithPermissionsQuery() : ICacheableQuery<List<RoleWithPermissionsDto>> //[cite: 26]
{
    public string CacheKey => "Roles:AllWithPermissions"; //[cite: 26]

    // 👇 التعديل: إطالة مدة الكاش
    public TimeSpan? SlidingExpiration => TimeSpan.FromHours(12);
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromHours(24);
}