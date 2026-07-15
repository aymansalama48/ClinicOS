using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Doctors;
using ClinicOS.Domain.Entities.Appointments;
using ClinicOS.Domain.Entities.Specializations;

namespace ClinicOS.Domain.Entities.Doctors;

/// <summary>
/// الطبيب
/// </summary>
public class Doctor : SoftDeleteEntity
{
    public Guid SpecializationId { get; set; }
    public Guid ApplicationUserId { get; set; } // حساب Identity إجباري

    public string? Bio { get; set; }
    public int? YearsOfExperience { get; set; }
    public decimal ConsultationFee { get; set; }
    public decimal UrgentSurchargeFee { get; set; } // رسوم العاجل
    public bool IsActive { get; set; } = true;

    // العلاقات
    public virtual Specialization Specialization { get; set; } = null!;
    public virtual ICollection<DoctorAvailability> Availabilities { get; set; } = new List<DoctorAvailability>();
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}