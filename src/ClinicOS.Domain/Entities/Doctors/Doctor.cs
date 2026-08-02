using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Entities.Appointments;
using ClinicOS.Domain.Entities.Specializations;

namespace ClinicOS.Domain.Entities.Doctors;

public class Doctor : SoftDeleteEntity
{
  

    public Guid SpecializationId { get; set; }
    public Guid ApplicationUserId { get; set; }
    public string? Bio { get; set; }
    public int? YearsOfExperience { get; set; }
    public decimal ConsultationFee { get; set; }
    public decimal UrgentSurchargeFee { get; set; }
    public bool IsActive { get; set; }

    public virtual Specialization Specialization { get; set; } = null!;
    public virtual ICollection<DoctorAvailability> Availabilities { get; set; } = new List<DoctorAvailability>();
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    protected Doctor() { }

    private Doctor(Guid applicationUserId, Guid specializationId, string? bio, int? yearsOfExperience, decimal consultationFee, decimal urgentSurchargeFee)
    {
        ApplicationUserId = applicationUserId;
        SpecializationId = specializationId;
        Bio = bio;
        YearsOfExperience = yearsOfExperience;
        ConsultationFee = consultationFee;
        UrgentSurchargeFee = urgentSurchargeFee;
        IsActive = true;
    }
    // 👇 دالة الإنشاء الآمنة (Factory Method)
    public static Doctor Create(Guid applicationUserId, Guid specializationId, string? bio, int? yearsOfExperience, decimal consultationFee, decimal urgentSurchargeFee)
    {
        if (consultationFee < 0) throw new ArgumentException("سعر الكشف لا يمكن أن يكون بالسالب.");
        if (urgentSurchargeFee < 0) throw new ArgumentException("رسوم الكشف العاجل لا يمكن أن تكون بالسالب.");
        if (yearsOfExperience.HasValue && yearsOfExperience.Value < 0) throw new ArgumentException("سنوات الخبرة غير صالحة.");

        return new Doctor(applicationUserId, specializationId, bio, yearsOfExperience, consultationFee, urgentSurchargeFee);
    }

    // 👇 دالة التعديل
    public void UpdateProfile(Guid specializationId, string? bio, int? yearsOfExperience, decimal consultationFee, decimal urgentSurchargeFee)
    {
        if (consultationFee < 0 || urgentSurchargeFee < 0) throw new ArgumentException("الرسوم غير صالحة.");

        SpecializationId = specializationId;
        Bio = bio;
        YearsOfExperience = yearsOfExperience;
        ConsultationFee = consultationFee;
        UrgentSurchargeFee = urgentSurchargeFee;
    }
}