using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Common.Errors.Doctors;

/// <summary>
/// الأخطاء الخاصة بالأطباء ومواعيدهم.
/// </summary>
public static class DoctorErrors
{
    public static readonly Error NotFound = new(
        "DOCTOR_NOT_FOUND",
        "الطبيب المطلوب غير موجود.",
        ErrorType.NotFound);

    // 👇 إضافة خطأ تعارض المواعيد هنا
    public static readonly Error AvailabilityConflict = new(
        "DOCTOR_AVAILABILITY_CONFLICT",
        "يوجد موعد مسجل مسبقاً لهذا الطبيب في نفس اليوم والفترة المحددة.",
        ErrorType.Conflict);

    public static readonly Error UserAlreadyDoctor = new(
    "DOCTOR_ALREADY_EXISTS",
    "هذا المستخدم مسجل كطبيب بالفعل في النظام.",
    ErrorType.Conflict);
    // 👇 الأخطاء الجديدة الخاصة بالبروفايل
    public static readonly Error ProfileAlreadyExists = new(
        "DOCTOR_PROFILE_ALREADY_EXISTS",
        "تم استكمال بيانات هذا الحساب مسبقاً.",
        ErrorType.Conflict);

    public static readonly Error ProfileNotFound = new(
        "DOCTOR_PROFILE_NOT_FOUND",
        "يجب استكمال البيانات الشخصية والمهنية أولاً.",
        ErrorType.NotFound);

}