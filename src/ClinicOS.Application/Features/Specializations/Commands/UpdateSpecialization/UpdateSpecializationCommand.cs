using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;
using System;
using System.Collections.Generic;

namespace ClinicOS.Application.Features.Specializations.Commands.UpdateSpecialization;

[Permission(Permissions.Specializations.Update)]
public sealed record UpdateSpecializationCommand(
    Guid SpecializationId,
    string Name,
    string? Description
) : ICommand<Guid>, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => [
        "specializations-list",
        $"specialization-details-{SpecializationId}"
    ];
}