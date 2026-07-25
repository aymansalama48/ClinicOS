using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Common.Errors.Identity;
public static class PasswordErrors
{
    public static readonly Error UserNotFound = new(
        "Password.UserNotFound",
        "المستخدم غير موجود.",
        ErrorType.NotFound);

    public static readonly Error IncorrectCurrentPassword = new(
        "Password.IncorrectCurrentPassword",
        "كلمة المرور الحالية غير صحيحة.",
        ErrorType.Validation);

    public static readonly Error ChangeFailed = new(
        "Password.ChangeFailed",
        "فشل تغيير كلمة المرور، تأكد أن الكلمة الجديدة تحقق الشروط المطلوبة.",
        ErrorType.Validation);

    public static readonly Error ResetFailed = new(
        "Password.ResetFailed",
        "فشلت إعادة تعيين كلمة المرور، الرابط غير صالح أو منتهي الصلاحية.",
        ErrorType.Validation);
}