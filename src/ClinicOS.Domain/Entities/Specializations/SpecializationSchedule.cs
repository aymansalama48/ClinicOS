using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Enums;

namespace ClinicOS.Domain.Entities.Specializations;

public class SpecializationSchedule : SoftDeleteEntity
{
    public Guid SpecializationId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public PeriodType Period { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public bool IsActive { get; private set; }

    public virtual Specialization Specialization { get; private set; } = null!;

    private SpecializationSchedule() { }

    internal SpecializationSchedule(Guid specializationId, DayOfWeek dayOfWeek, PeriodType period, TimeOnly startTime, TimeOnly endTime)
    {
        SpecializationId = specializationId;
        DayOfWeek = dayOfWeek;
        Period = period;
        StartTime = startTime;
        EndTime = endTime;
        IsActive = true;
    }

    internal static SpecializationSchedule Create(Guid specializationId, DayOfWeek dayOfWeek, PeriodType period, TimeOnly startTime, TimeOnly endTime)
    {
        return new SpecializationSchedule(specializationId, dayOfWeek, period, startTime, endTime);
    }

    internal void Deactivate() => IsActive = false;
    internal void Activate() => IsActive = true;

    internal void UpdateDetails(DayOfWeek dayOfWeek, PeriodType period, TimeOnly startTime, TimeOnly endTime)
    {
        DayOfWeek = dayOfWeek;
        Period = period;
        StartTime = startTime;
        EndTime = endTime;
    }
}