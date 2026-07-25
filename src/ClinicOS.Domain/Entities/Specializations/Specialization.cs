using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Entities.Doctors;
using ClinicOS.Domain.Entities.Receptionists;

namespace ClinicOS.Domain.Entities.Specializations;

/// <summary>
/// التخصص الطبي
/// </summary>
public class Specialization(string name, string? description) : SoftDeleteEntity
{
    public string Name { get; set; } = name;
    public string? Description { get; set; } = description;

    // العلاقات
    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    public virtual ICollection<Receptionist> Receptionists { get; set; } = new List<Receptionist>();
    public virtual ICollection<SpecializationSchedule> Schedules { get; set; } = new List<SpecializationSchedule>();
}