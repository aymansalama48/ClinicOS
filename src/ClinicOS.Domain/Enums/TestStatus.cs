namespace ClinicOS.Domain.Enums;

/// <summary>
/// حالة الطلب المخبري
/// </summary>
public enum TestStatus
{
    Ordered,   // طلب إجراء التحليل
    Completed  // تم رفع النتيجة
}