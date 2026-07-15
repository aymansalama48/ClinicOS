namespace ClinicOS.Domain.Enums;

/// <summary>
/// نوع الرسوم المدفوعة
/// </summary>
public enum PaymentType
{
    Consultation,    // رسوم الكشف الأساسية
    UrgentSurcharge, // رسوم العاجل الإضافية
    FollowUp         // رسوم المتابعة
}