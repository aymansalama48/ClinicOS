using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Enums;

namespace ClinicOS.Domain.Entities.Doctors;

/// <summary>
/// توفر الطبيب في أيام وفترات محددة
/// </summary>
public class DoctorAvailability : BaseEntity
{
    public Guid DoctorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public PeriodType Period { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int MaxPatients { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;
}