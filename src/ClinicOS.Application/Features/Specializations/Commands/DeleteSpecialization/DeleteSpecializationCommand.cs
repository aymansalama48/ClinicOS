using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;
using System;
using System.Collections.Generic;

namespace ClinicOS.Application.Features.Specializations.Commands.DeleteSpecialization;

[Permission(Permissions.Specializations.Delete)]
public sealed record DeleteSpecializationCommand(Guid Id) : ICommand<bool>, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => [
        "specializations-list",
        $"specialization-details-{Id}"
    ];
}