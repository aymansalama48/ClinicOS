using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Domain.Errors.Doctors;

// تجميع كافة أخطاء البزنس الخاصة بالطبيب لسهولة الإدارة وتجنب النصوص المباشرة
public static class DoctorErrors
{
    public static readonly Error NegativeFee = new(
        "Doctor.NegativeFee",
        "رسوم الكشف والرسوم المستعجلة لا يمكن أن تكون بالسالب.",
        ErrorType.Validation);

    public static readonly Error InvalidWorkingHours = new(
        "DoctorAvailability.InvalidWorkingHours",
        "وقت الانتهاء يجب أن يكون بعد وقت البداية.",
        ErrorType.Validation);

    public static readonly Error InvalidYearsOfExperience = new(
        "Doctor.InvalidYearsOfExperience",
        "سنوات الخبرة لا يمكن أن تكون بالسالب.",
        ErrorType.Validation);
}