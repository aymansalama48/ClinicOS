namespace ClinicOS.Application.Features.Accounts.PermissionManagement.Queries.GetAllPermissions;

using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization.Models;
using ClinicOS.Domain.Constants;
using System;

[Permission(Permissions.Users.View)]
public sealed record GetAllPermissionsQuery() : ICacheableQuery<List<PermissionDto>> //[cite: 27]
{
    public string CacheKey => "Permissions:All"; //[cite: 27]

    // 👇 التعديل: إطالة مدة الكاش (الصلاحيات الأساسية للنظام مش بتتغير تقريباً)
    public TimeSpan? SlidingExpiration => TimeSpan.FromDays(1);
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromDays(7);
}