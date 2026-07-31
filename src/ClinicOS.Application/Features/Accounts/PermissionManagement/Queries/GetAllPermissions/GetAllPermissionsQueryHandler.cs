using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization.Models;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Features.Accounts.PermissionManagement.Queries.GetAllPermissions
{
    public sealed class GetAllPermissionsQueryHandler(
        IPermissionManagementService permissionManagementService)
        : IQueryHandler<GetAllPermissionsQuery, List<PermissionDto>>
    {
        public async Task<Result<List<PermissionDto>>> Handle(
            GetAllPermissionsQuery request,
            CancellationToken cancellationToken)
        {
            return await permissionManagementService.GetAllPermissionsAsync(cancellationToken);
        }
    }
}
