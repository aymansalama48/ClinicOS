using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Common.Errors.Identity;

// تجميع كافة أخطاء البزنس الخاصة بالـ OTP لسهولة الإدارة وتجنب النصوص المباشرة
public static class OtpErrors
{
    public static readonly Error NotFound = new(
        "Otp.NotFound",
        "مفيش كود تحقق صالح لهذا الرقم، من فضلك اطلب كود جديد.",
        ErrorType.NotFound);

    public static readonly Error Expired = new(
        "Otp.Expired",
        "انتهت صلاحية كود التحقق.",
        ErrorType.Validation);

    public static readonly Error InvalidCode = new(
        "Otp.InvalidCode",
        "كود التحقق غير صحيح.",
        ErrorType.Validation);

    public static readonly Error MaxAttemptsExceeded = new(
        "Otp.MaxAttemptsExceeded",
        "تجاوزت الحد الأقصى لعدد المحاولات، من فضلك اطلب كود جديد.",
        ErrorType.Validation);

    public static readonly Error ResendTooSoon = new(
        "Otp.ResendTooSoon",
        "من فضلك انتظر قليلاً قبل طلب كود جديد.",
        ErrorType.Validation);
}