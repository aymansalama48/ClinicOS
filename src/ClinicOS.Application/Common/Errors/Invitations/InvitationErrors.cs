namespace ClinicOS.Application.Common.Errors.Invitations;

using ClinicOS.Domain.Common.Results;

/// <summary>
/// الأخطاء الخاصة بنظام دعوات الموظفين (Staff Invitations)
/// </summary>
public static class InvitationErrors
{
    public static readonly Error NotFound = new(
        "INVITATION_NOT_FOUND",
        "دعوة الموظف غير موجودة.",
        ErrorType.NotFound);

    public static readonly Error AlreadyUsed = new(
        "INVITATION_ALREADY_USED",
        "تم استخدام رابط هذه الدعوة مسبقاً.",
        ErrorType.Validation);

    public static readonly Error Expired = new(
        "INVITATION_EXPIRED",
        "انتهت صلاحية رابط الدعوة.",
        ErrorType.Validation);

    public static readonly Error InvalidOrExpired = new(
        "INVITATION_INVALID_OR_EXPIRED",
        "رابط الدعوة غير صالح أو منتهي الصلاحية.",
        ErrorType.Validation);
}