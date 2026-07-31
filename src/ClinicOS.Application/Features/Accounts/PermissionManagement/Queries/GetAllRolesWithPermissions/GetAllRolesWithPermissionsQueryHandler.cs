using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization.Models;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Features.Accounts.PermissionManagement.Queries.GetAllRolesWithPermissions
{
    public sealed class GetAllRolesWithPermissionsQueryHandler(
        IPermissionManagementService permissionManagementService)
        : IQueryHandler<GetAllRolesWithPermissionsQuery, List<RoleWithPermissionsDto>>
    {
        public async Task<Result<List<RoleWithPermissionsDto>>> Handle(
            GetAllRolesWithPermissionsQuery request,
            CancellationToken cancellationToken)
        {
            return await permissionManagementService.GetAllRolesWithPermissionsAsync(cancellationToken);
        }
    }
}
