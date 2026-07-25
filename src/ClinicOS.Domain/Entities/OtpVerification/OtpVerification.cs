using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Entities.Appointments;
using ClinicOS.Domain.Enums;

namespace ClinicOS.Domain.Entities.OtpVerification;

/// <summary>
/// سجل مستقل للتحقق عبر OTP
/// </summary>
public class OtpVerification : BaseEntity
{
    public string Phone { get; set; } = string.Empty;

    /// الكود بيتخزن Hashed دايمًا (HMAC)، مش نص عادي — عشان حتى لو حد وصل للداتابيز میقدرش يستخدمه
    public string CodeHash { get; set; } = string.Empty;

    public DateTime Expiry { get; set; }
    public DateTime NextResendAllowedAtUtc { get; set; }

    public OtpPurpose Purpose { get; set; } = OtpPurpose.AppointmentBooking;

    /// اتستهلك = نجح التحقق، أو اتقفل بسبب Max Attempts، أو اتلغى لصدور كود جديد بدله
    public bool IsConsumed { get; set; } = false;
    public DateTime? VerifiedAt { get; set; }

    public int AttemptsCount { get; set; } = 0;
    public int MaxAttempts { get; set; } = 5;
    public bool IsMaxAttemptsReached => AttemptsCount >= MaxAttempts;

    // ربط اختياري بالحجز في حال كان الغرض هو تأكيد حجز معلق Pending
    public Guid? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }
}