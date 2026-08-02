using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Enums;
using System;

namespace ClinicOS.Domain.Entities.Doctors;

/// <summary>
/// توفر الطبيب في أيام وفترات محددة
/// </summary>
public sealed class DoctorAvailability : AuditableEntity // 👈 توحيد الوراثة لمتابعة الإنشاء والتعديل
{
    public Guid DoctorId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public PeriodType Period { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public int? MaxPatients { get; private set; }

    public Doctor Doctor { get; private set; } = null!;

    private DoctorAvailability() { }

    private DoctorAvailability(
        Guid doctorId,
        DayOfWeek dayOfWeek,
        PeriodType period,
        TimeOnly startTime,
        TimeOnly endTime,
        int? maxPatients)
    {
        Id = Guid.CreateVersion7();
        DoctorId = doctorId;
        DayOfWeek = dayOfWeek;
        Period = period;
        StartTime = startTime;
        EndTime = endTime;
        MaxPatients = maxPatients;
    }

    public static DoctorAvailability Create(
        Guid doctorId,
        DayOfWeek dayOfWeek,
        PeriodType period,
        TimeOnly startTime,
        TimeOnly endTime,
        int? maxPatients)
    {
        if (endTime <= startTime)
        {
            throw new ArgumentException("وقت النهاية يجب أن يكون بعد وقت البداية.");
        }

        if (maxPatients.HasValue && maxPatients.Value <= 0)
        {
            throw new ArgumentException("الحد الأقصى للمرضى يجب أن يكون رقماً أكبر من الصفر.");
        }

        return new DoctorAvailability(doctorId, dayOfWeek, period, startTime, endTime, maxPatients);
    }
}