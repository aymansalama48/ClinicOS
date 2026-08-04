using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;
using ClinicOS.Domain.Enums; // مسار الـ Enums الخاص بك

namespace ClinicOS.Application.Features.Specializations.Commands.AddSpecializationSchedule;

[Permission(Permissions.Specializations.Update)] // نعتبرها جزء من تحديث التخصص
public sealed record AddSpecializationScheduleCommand(
    Guid SpecializationId,
    DayOfWeek DayOfWeek,
    PeriodType Period,
    TimeOnly StartTime,
    TimeOnly EndTime
) : ICommand<Guid>, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => [
        "specializations-list",
        $"specialization-details-{SpecializationId}"
    ];
}