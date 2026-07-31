using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Common.Results;


namespace ClinicOS.Application.Features.Accounts.PermissionManagement.Commands.UpdateRolePermissions
{
    public sealed class UpdateRolePermissionsCommandHandler(
        IPermissionManagementService permissionManagementService)
        : ICommandHandler<UpdateRolePermissionsCommand>
    {
        public async Task<Result> Handle(
            UpdateRolePermissionsCommand request,
            CancellationToken cancellationToken)
        {
            return await permissionManagementService.UpdateRolePermissionsAsync(
                request.RoleId,
                request.PermissionIds,
                cancellationToken);
        }
    }
}
