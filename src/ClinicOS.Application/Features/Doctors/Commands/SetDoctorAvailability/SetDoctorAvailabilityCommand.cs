using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Enums;
using System;

namespace ClinicOS.Application.Features.Doctors.Commands.SetDoctorAvailability;

// استخدمنا Record لأنه الأفضل للـ Commands (Immutable)
public sealed record SetDoctorAvailabilityCommand(
    Guid DoctorId,
    DayOfWeek DayOfWeek,
    PeriodType Period,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int? MaxPatients
) : ICommand<Guid>; // بيرجع الـ ID بتاع الموعد اللي اتكريت