using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Common.Results;


namespace ClinicOS.Application.Features.Accounts.PermissionManagement.Commands.AssignPermissionToRole
{
    public sealed class AssignPermissionToRoleCommandHandler(
       IPermissionManagementService permissionManagementService)
       : ICommandHandler<AssignPermissionToRoleCommand>
    {
        public async Task<Result> Handle(
            AssignPermissionToRoleCommand request,
            CancellationToken cancellationToken)
        {
            return await permissionManagementService.AssignPermissionToRoleAsync(
                request.RoleId,
                request.PermissionId,
                cancellationToken);
        }
    }
}
