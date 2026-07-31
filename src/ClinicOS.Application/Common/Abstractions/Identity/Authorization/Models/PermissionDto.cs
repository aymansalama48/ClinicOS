using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Common.Abstractions.Identity.Authorization.Models
{
    // DTO لتمثيل الصلاحية الواحدة
    public record PermissionDto(
        Guid Id,
        string Name,
        string DisplayName,
        string Module,
        string? Description);
}
