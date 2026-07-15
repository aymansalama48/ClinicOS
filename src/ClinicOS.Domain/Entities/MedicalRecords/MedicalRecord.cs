using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Entities.Appointments;
using ClinicOS.Domain.Entities.Prescriptions;
using static System.Net.Mime.MediaTypeNames;

namespace ClinicOS.Domain.Entities.MedicalRecords;

/// <summary>
/// السجل الطبي للزيارة
/// </summary>
public class MedicalRecord : AuditableEntity
{
    public Guid AppointmentId { get; set; }

    public string? Diagnosis { get; set; }
    public string? Notes { get; set; }
    public Guid? CreatedByDoctorId { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}