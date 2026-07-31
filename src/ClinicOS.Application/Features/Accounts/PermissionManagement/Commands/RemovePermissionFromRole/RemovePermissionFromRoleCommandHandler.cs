using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Features.Accounts.PermissionManagement.Commands.RemovePermissionFromRole
{
    public sealed class RemovePermissionFromRoleCommandHandler(
        IPermissionManagementService permissionManagementService)
        : ICommandHandler<RemovePermissionFromRoleCommand>
    {
        public async Task<Result> Handle(
            RemovePermissionFromRoleCommand request,
            CancellationToken cancellationToken)
        {
            return await permissionManagementService.RemovePermissionFromRoleAsync(
                request.RoleId,
                request.PermissionId,
                cancellationToken);
        }
    }
}
