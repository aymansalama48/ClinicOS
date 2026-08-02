using ClinicOS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Features.Specializations.Shared
{
    // DTO لمواعيد التخصص (العيادة)
    public record SpecializationScheduleResponse(
        Guid Id,
        DayOfWeek DayOfWeek,
        PeriodType Period,
        TimeOnly StartTime,
        TimeOnly EndTime,
        bool IsActive
    );
}
