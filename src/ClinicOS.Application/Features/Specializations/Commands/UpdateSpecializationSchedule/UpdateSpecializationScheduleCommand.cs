using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;
using ClinicOS.Domain.Enums;
using System;

namespace ClinicOS.Application.Features.Specializations.Commands.UpdateSpecializationSchedule;

[Permission(Permissions.Specializations.Update)]
public sealed record UpdateSpecializationScheduleCommand(
    Guid SpecializationId,
    Guid ScheduleId,
    DayOfWeek DayOfWeek,
    PeriodType Period,
    TimeOnly StartTime,
    TimeOnly EndTime
) : ICommand<bool>, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => [
        "specializations-list",
        $"specialization-details-{SpecializationId}"
    ];
}
