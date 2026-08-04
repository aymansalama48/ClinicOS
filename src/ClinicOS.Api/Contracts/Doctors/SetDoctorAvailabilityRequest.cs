using ClinicOS.Domain.Enums;

namespace ClinicOS.Api.Contracts.Doctors;

public sealed record SetDoctorAvailabilityRequest(
    DayOfWeek DayOfWeek,
    PeriodType Period,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int? MaxPatients);
