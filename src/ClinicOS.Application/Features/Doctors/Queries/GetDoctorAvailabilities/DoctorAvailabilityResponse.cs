using ClinicOS.Domain.Enums;
using System;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctorAvailabilities;

public sealed record DoctorAvailabilityResponse(
    Guid Id,
    DayOfWeek DayOfWeek,
    PeriodType Period,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int? MaxPatients
);