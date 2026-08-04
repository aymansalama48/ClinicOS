using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Entities.Doctors;
using ClinicOS.Domain.Entities.Receptionists;
using ClinicOS.Domain.Entities.Specializations.Events; // مسار الـ Events اللي عملناه
using ClinicOS.Domain.Entities.Storage;
using ClinicOS.Domain.Enums;

namespace ClinicOS.Domain.Entities.Specializations;

public class Specialization : SoftDeleteEntity, IHasAttachments
{
    private readonly List<SpecializationSchedule> _schedules = new();
    private readonly List<Doctor> _doctors = new();
    private readonly List<Receptionist> _receptionists = new();

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    // الربط مع خدمة الملفات المعزولة (جدول Attachments)
    public Guid? IconAttachmentId { get; private set; }
    public virtual Attachment? IconAttachment { get; private set; }

    public virtual IReadOnlyCollection<SpecializationSchedule> Schedules => _schedules.AsReadOnly();
    public virtual IReadOnlyCollection<Doctor> Doctors => _doctors.AsReadOnly();
    public virtual IReadOnlyCollection<Receptionist> Receptionists => _receptionists.AsReadOnly();

    private Specialization() { }

    private Specialization(string name, string? description, Guid? iconAttachmentId)
    {
        Name = name;
        Description = description;
        IconAttachmentId = iconAttachmentId;
        IsActive = true;
    }

    public static Specialization Create(string name, string? description, Guid? iconAttachmentId = null)
    {
        var specialization = new Specialization(name, description, iconAttachmentId);

        // تسجيل حدث الإنشاء
        specialization.AddDomainEvent(new SpecializationCreatedDomainEvent(specialization.Id));

        return specialization;
    }

    public void UpdateDetails(string name, string? description, Guid? iconAttachmentId)
    {
        Name = name;
        Description = description;
        IconAttachmentId = iconAttachmentId;
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        AddDomainEvent(new SpecializationDeactivatedDomainEvent(Id));
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        AddDomainEvent(new SpecializationActivatedDomainEvent(Id));
    }

    public void AddSchedule(DayOfWeek dayOfWeek, PeriodType period, TimeOnly startTime, TimeOnly endTime)
    {
        var schedule = SpecializationSchedule.Create(Id, dayOfWeek, period, startTime, endTime);
        _schedules.Add(schedule);
    }

    public void RemoveSchedule(Guid scheduleId)
    {
        var schedule = _schedules.FirstOrDefault(s => s.Id == scheduleId);
        if (schedule != null)
        {
            _schedules.Remove(schedule);
        }
    }

    public void UpdateSchedule(Guid scheduleId, DayOfWeek dayOfWeek, PeriodType period, TimeOnly startTime, TimeOnly endTime)
    {
        var schedule = _schedules.FirstOrDefault(s => s.Id == scheduleId);
        if (schedule != null)
        {
            schedule.UpdateDetails(dayOfWeek, period, startTime, endTime);
        }
    }

    public void ToggleScheduleStatus(Guid scheduleId)
    {
        var schedule = _schedules.FirstOrDefault(s => s.Id == scheduleId);
        if (schedule != null)
        {
            if (schedule.IsActive)
                schedule.Deactivate();
            else
                schedule.Activate();
        }
    }
}