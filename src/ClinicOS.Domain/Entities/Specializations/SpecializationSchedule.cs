using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Enums;

namespace ClinicOS.Domain.Entities.Specializations;

/// <summary>
/// جدول أوقات عمل العيادة الخاصة بالتخصص
/// </summary>
public class SpecializationSchedule : BaseEntity
{
    public Guid SpecializationId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public PeriodType Period { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual Specialization Specialization { get; set; } = null!;
}