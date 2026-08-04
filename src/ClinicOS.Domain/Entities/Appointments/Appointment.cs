using ClinicOS.Domain.Enums;
using ClinicOS.Domain.Entities.Doctors;
using ClinicOS.Domain.Entities.Patients;
using ClinicOS.Domain.Entities.MedicalRecords;
using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Entities.Appointments.Events;

namespace ClinicOS.Domain.Entities.Appointments;

/// <summary>
/// الموعد (الزيارة) - أهم كيان في النظام
/// تم تطبيق مبادئ (DDD) بجعل الخصائص مغلقة للإضافة من الخارج، واستخدام دوال لتعديل الحالة.
/// </summary>
public class Appointment : AuditableEntity
{
    public Guid DoctorId { get; private set; }
    public Guid PatientId { get; private set; }

    public DateOnly AppointmentDate { get; private set; }
    public PeriodType Period { get; private set; }

    public AppointmentType Type { get; private set; } = AppointmentType.New;
    public PriorityLevel Priority { get; private set; } = PriorityLevel.Regular;
    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Pending;

    // بيانات الحجز المؤقتة (للمرضى غير المسجلين)
    public string? BookingName { get; private set; }
    public string? BookingPhone { get; private set; }

    // الطابور (يُحدد عند Check-in)
    public int? QueueNumber { get; private set; }
    public DateTime? QueuedAt { get; private set; }

    // توقيتات تغيير الحالة
    public DateTime? CheckedInAt { get; private set; }
    public DateTime? InProgressAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? PendingExpiresAt { get; private set; } // وقت انتهاء صلاحية الحالة Pending

    // إجمالي المبلغ (مجموع الرسوم)
    public decimal? TotalAmount { get; private set; }

    // رسوم المتابعة (يُحدد عند إنشاء موعد متابعة)
    public decimal? FollowUpFee { get; private set; }

    public string? Notes { get; private set; }

    // Concurrency
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    // العلاقات
    public virtual Doctor Doctor { get; private set; } = null!;
    public virtual Patient Patient { get; private set; } = null!;
    public virtual MedicalRecord? MedicalRecord { get; private set; }
    public virtual ICollection<Payment> Payments { get; private set; } = new List<Payment>();

    // Entity Framework Constructor
    protected Appointment() { }

    /// <summary>
    /// دالة إنشاء الموعد الأولي
    /// </summary>
    public static Appointment Create(Guid doctorId, Guid patientId, DateOnly appointmentDate, PeriodType period, AppointmentType type, string? notes)
    {
        return new Appointment
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            PatientId = patientId,
            AppointmentDate = appointmentDate,
            Period = period,
            Type = type,
            Status = AppointmentStatus.Pending,
            Notes = notes
        };
    }

    /// <summary>
    /// تأكيد الموعد وإطلاق حدث التأكيد
    /// </summary>
    public void Confirm()
    {
        if (Status != AppointmentStatus.Pending)
            throw new InvalidOperationException("Can only confirm pending appointments.");

        Status = AppointmentStatus.Confirmed;
        AddDomainEvent(new AppointmentConfirmedEvent(Id));
    }

    /// <summary>
    /// تسجيل وصول المريض للعيادة
    /// </summary>
    public void CheckIn(int queueNumber)
    {
        if (Status != AppointmentStatus.Confirmed)
            throw new InvalidOperationException("Can only check-in confirmed appointments.");

        Status = AppointmentStatus.CheckedIn;
        QueueNumber = queueNumber;
        QueuedAt = DateTime.UtcNow;
        CheckedInAt = DateTime.UtcNow;
    }

    /// <summary>
    /// بدء الكشف من قبل الطبيب
    /// </summary>
    public void StartProgress()
    {
        if (Status != AppointmentStatus.CheckedIn)
            throw new InvalidOperationException("Patient must be checked-in to start progress.");

        Status = AppointmentStatus.InProgress;
        InProgressAt = DateTime.UtcNow;
    }

    /// <summary>
    /// إنهاء الكشف وإطلاق حدث الاكتمال
    /// </summary>
    public void Complete(decimal totalAmount)
    {
        if (Status != AppointmentStatus.InProgress)
            throw new InvalidOperationException("Cannot complete an appointment that is not in progress.");

        Status = AppointmentStatus.Completed;
        TotalAmount = totalAmount;
        CompletedAt = DateTime.UtcNow;
        
        AddDomainEvent(new AppointmentCompletedEvent(Id));
    }

    public void Cancel(string notes)
    {
        Status = AppointmentStatus.Cancelled;
        Notes = notes;
    }

    public void SetMedicalRecord(MedicalRecord record)
    {
        MedicalRecord = record;
    }
}