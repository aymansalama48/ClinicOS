namespace ClinicOS.Application.Features.Accounts.PermissionManagement.Commands.AssignPermissionToRole;

using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;

[Permission(Permissions.SettingsManage)]
public sealed record AssignPermissionToRoleCommand(
    Guid RoleId,
    Guid PermissionId) : ICommand, ICacheInvalidatorCommand
{
    // نمسح الكاش الخاص بكل الأدوار، والكاش الخاص بهذا الدور تحديداً
    public IReadOnlyCollection<string> CacheKeys => [
        "Roles:AllWithPermissions",
        $"Roles:{RoleId}:Permissions"
    ];
}