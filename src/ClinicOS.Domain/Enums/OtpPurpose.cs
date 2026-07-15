namespace ClinicOS.Domain.Enums;

/// <summary>
/// الغرض من التحقق عبر OTP
/// </summary>
public enum OtpPurpose
{
    AppointmentBooking,   // تأكيد حجز جديد
    ViewAppointments,     // مشاهدة الحجوزات
    LinkAccount,          // ربط حساب دائم
    VerifyPhone           // تغيير رقم الهاتف أو تأكيده
}