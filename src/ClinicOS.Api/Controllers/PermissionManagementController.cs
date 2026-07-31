namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Features.Accounts.PermissionManagement.Commands.AssignPermissionToRole;
using ClinicOS.Application.Features.Accounts.PermissionManagement.Commands.RemovePermissionFromRole;
using ClinicOS.Application.Features.Accounts.PermissionManagement.Commands.UpdateRolePermissions;
using ClinicOS.Application.Features.Accounts.PermissionManagement.Queries.GetAllPermissions;
using ClinicOS.Application.Features.Accounts.PermissionManagement.Queries.GetAllRolesWithPermissions;
using ClinicOS.Application.Features.Accounts.PermissionManagement.Queries.GetRolePermissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize] // تأكيد أن المستخدم مسجل دخول كطبقة حماية أولى
[Route("api/permission-management")]
public class PermissionManagementController : BaseApiController
{
    [HttpGet("permissions")]
    public async Task<IResult> GetAllPermissions(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllPermissionsQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("roles")]
    public async Task<IResult> GetAllRolesWithPermissions(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllRolesWithPermissionsQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("roles/{roleId}")]
    public async Task<IResult> GetRolePermissions(Guid roleId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetRolePermissionsQuery(roleId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("roles/{roleId}/permissions/{permissionId}/assign")]
    public async Task<IResult> AssignPermissionToRole(Guid roleId, Guid permissionId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new AssignPermissionToRoleCommand(roleId, permissionId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("roles/{roleId}/permissions/{permissionId}/remove")]
    public async Task<IResult> RemovePermissionFromRole(Guid roleId, Guid permissionId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new RemovePermissionFromRoleCommand(roleId, permissionId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("roles/{roleId}/permissions/update")]
    public async Task<IResult> UpdateRolePermissions(Guid roleId, [FromBody] List<Guid> permissionIds, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new UpdateRolePermissionsCommand(roleId, permissionIds), cancellationToken);
        return HandleResult(result);
    }
}