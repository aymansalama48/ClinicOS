namespace ClinicOS.Application.Features.Accounts.PermissionManagement.Queries.GetRolePermissions;

using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization.Models;
using ClinicOS.Domain.Constants;
using System;

[Permission(Permissions.Users.View)]
public sealed record GetRolePermissionsQuery(Guid RoleId) : ICacheableQuery<RoleWithPermissionsDto> //[cite: 25]
{
    public string CacheKey => $"Roles:{RoleId}:Permissions"; //[cite: 25]

    // 👇 التعديل: إطالة مدة الكاش
    public TimeSpan? SlidingExpiration => TimeSpan.FromHours(12);
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromHours(24);
}