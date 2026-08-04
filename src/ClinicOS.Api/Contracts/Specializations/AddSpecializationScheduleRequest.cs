namespace ClinicOS.Api.Contracts.Specializations;

using ClinicOS.Domain.Enums;
using System;

public sealed record AddSpecializationScheduleRequest(
    DayOfWeek DayOfWeek,
    PeriodType Period,
    TimeOnly StartTime,
    TimeOnly EndTime
);
