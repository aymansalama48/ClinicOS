using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization.Models;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Features.Accounts.PermissionManagement.Queries.GetRolePermissions
{
    public sealed class GetRolePermissionsQueryHandler(
        IPermissionManagementService permissionManagementService)
        : IQueryHandler<GetRolePermissionsQuery, RoleWithPermissionsDto>
    {
        public async Task<Result<RoleWithPermissionsDto>> Handle(
            GetRolePermissionsQuery request,
            CancellationToken cancellationToken)
        {
            return await permissionManagementService.GetRolePermissionsAsync(request.RoleId, cancellationToken);
        }
    }
}
