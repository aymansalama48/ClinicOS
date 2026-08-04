using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;
using System;

namespace ClinicOS.Application.Features.Specializations.Commands.ToggleSpecializationSchedule;

[Permission(Permissions.Specializations.Update)]
public sealed record ToggleSpecializationScheduleCommand(
    Guid SpecializationId,
    Guid ScheduleId
) : ICommand<bool>, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => [
        "specializations-list",
        $"specialization-details-{SpecializationId}"
    ];
}
