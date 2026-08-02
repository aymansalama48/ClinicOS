using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Features.Specializations.Shared
{
    // DTO لتفاصيل التخصص بالكامل (مع المواعيد)
    public record SpecializationDetailsResponse(
        Guid Id,
        string Name,
        string? Description,
        List<SpecializationScheduleResponse> Schedules
    );
}
