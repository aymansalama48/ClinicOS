using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Enums;

namespace ClinicOS.Domain.Entities.Appointments;

/// <summary>
/// عملية دفع مرتبطة بموعد (يمكن أن تكون متعددة)
/// </summary>
public class Payment : AuditableEntity
{
    public Guid AppointmentId { get; set; }

    public decimal Amount { get; set; }
    public PaymentType PaymentType { get; set; }
    public string? PaymentMethod { get; set; } // Cash, Card, إلخ
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime? PaymentDate { get; set; }
    public Guid? ReceivedBy { get; set; } // معرف المستخدم (Receptionist)

    public virtual Appointment Appointment { get; set; } = null!;
}