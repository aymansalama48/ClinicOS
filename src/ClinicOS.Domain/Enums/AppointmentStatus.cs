namespace ClinicOS.Domain.Enums;

/// <summary>
/// حالة الموعد خلال دورة حياته
/// </summary>
public enum AppointmentStatus
{
    Pending,       // انتظار التحقق من OTP
    Confirmed,     // تم تأكيد OTP
    CheckedIn,     // وصول المريض ودخول الطابور
    InProgress,    // قيد الكشف من قبل الطبيب
    AwaitingTests, // في انتظار نتائج التحاليل
    Completed,     // انتهى الكشف
    Cancelled      // ملغي
}