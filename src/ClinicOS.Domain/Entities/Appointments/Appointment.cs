using ClinicOS.Domain.Enums;
using ClinicOS.Domain.Entities.Doctors;
using ClinicOS.Domain.Entities.Patients;
using ClinicOS.Domain.Entities.MedicalRecords;
using ClinicOS.Domain.Common.Entities;

namespace ClinicOS.Domain.Entities.Appointments;

/// <summary>
/// الموعد (الزيارة) - أهم كيان في النظام
/// </summary>
public class Appointment : AuditableEntity
{
    public Guid DoctorId { get; set; }
    public Guid PatientId { get; set; }

    public DateOnly AppointmentDate { get; set; }
    public PeriodType Period { get; set; }

    public AppointmentType Type { get; set; } = AppointmentType.New;
    public PriorityLevel Priority { get; set; } = PriorityLevel.Regular;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    // بيانات الحجز المؤقتة (للمرضى غير المسجلين)
    public string? BookingName { get; set; }
    public string? BookingPhone { get; set; }

    // الطابور (يُحدد عند Check-in)
    public int? QueueNumber { get; set; }
    public DateTime? QueuedAt { get; set; }

    // توقيتات تغيير الحالة
    public DateTime? CheckedInAt { get; set; }
    public DateTime? InProgressAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? PendingExpiresAt { get; set; } // وقت انتهاء صلاحية الحالة Pending

    // إجمالي المبلغ (مجموع الرسوم)
    public decimal? TotalAmount { get; set; }

    // رسوم المتابعة (يُحدد عند إنشاء موعد متابعة)
    public decimal? FollowUpFee { get; set; }

    public string? Notes { get; set; }

    // Concurrency
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // العلاقات
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual Patient Patient { get; set; } = null!;
    public virtual MedicalRecord? MedicalRecord { get; set; }
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}