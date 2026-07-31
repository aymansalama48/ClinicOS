namespace ClinicOS.Application.Features.Accounts.PermissionManagement.Commands.UpdateRolePermissions;

using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;

[Permission(Permissions.SettingsManage)]
public sealed record UpdateRolePermissionsCommand(
    Guid RoleId,
    List<Guid> PermissionIds) : ICommand, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => [
        "Roles:AllWithPermissions",
        $"Roles:{RoleId}:Permissions"
    ];
}