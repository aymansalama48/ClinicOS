using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;
using ClinicOS.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ClinicOS.Application.Features.Doctors.Commands.SetDoctorAvailability;

[Permission(Permissions.Doctors.ManageAvailability)]
public sealed record SetDoctorAvailabilityCommand(
    Guid DoctorId,
    DayOfWeek DayOfWeek,
    PeriodType Period,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int? MaxPatients
) : ICommand<Guid>, ICacheInvalidatorCommand // 👈 الوراثة مزدوجة عشان يقبله الـ Handler ويدعم إبطال الكاش
{
    public IReadOnlyCollection<string> CacheKeys => [
        $"doctor-availability-{DoctorId}"
    ];
}