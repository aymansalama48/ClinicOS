using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Common.Errors.Receptionists;

public static class ReceptionistErrors
{
    public static readonly Error UserAlreadyReceptionist = new(
        "RECEPTIONIST_ALREADY_EXISTS",
        "هذا المستخدم مسجل كموظف استقبال بالفعل في النظام.",
        ErrorType.Conflict);

    public static readonly Error NotFound = new(
        "RECEPTIONIST_NOT_FOUND",
        "موظف الاستقبال المطلوب غير موجود.",
        ErrorType.NotFound);
    // 👇 الأخطاء الجديدة الخاصة بالبروفايل
    public static readonly Error ProfileAlreadyExists = new(
        "RECEPTIONIST_PROFILE_ALREADY_EXISTS",
        "تم استكمال بيانات هذا الحساب مسبقاً.",
        ErrorType.Conflict);

    public static readonly Error ProfileNotFound = new(
        "RECEPTIONIST_PROFILE_NOT_FOUND",
        "يجب استكمال البيانات الشخصية أولاً.",
        ErrorType.NotFound);
}