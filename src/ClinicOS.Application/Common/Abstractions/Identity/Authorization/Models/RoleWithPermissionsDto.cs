using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Common.Abstractions.Identity.Authorization.Models
{
    //  DTO لتمثيل الدور مع صلاحياته الحالية
    public record RoleWithPermissionsDto(
        Guid RoleId,
        string RoleName,
        string? Description,
        bool IsSystemRole,
        List<PermissionDto> Permissions);
}
