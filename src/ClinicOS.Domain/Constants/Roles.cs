namespace ClinicOS.Domain.Constants;

/// <summary>
/// الثوابت الخاصة بأدوار المستخدمين في النظام (Staff + Patient)
/// </summary>
public static class Roles
{
    /// <summary>
    /// المدير العام - لديه كافة الصلاحيات على النظام
    /// </summary>
    public const string Admin = "Admin";

    /// <summary>
    /// الطبيب - يمكنه إدارة المواعيد الخاصة به، والسجلات الطبية، والروشتات، والتحاليل
    /// </summary>
    public const string Doctor = "Doctor";

    /// <summary>
    /// موظف الاستقبال - يمكنه إدارة المواعيد، والدفع، والطابور، والتحقق من وصول المرضى
    /// </summary>
    public const string Receptionist = "Receptionist";

    /// <summary>
    /// المريض (صاحب حساب دائم) - يمكنه عرض حجوزاته وملفاته الشخصية
    /// </summary>
    public const string Patient = "Patient";

    /// <summary>
    /// قائمة بكل الأدوار المتاحة في النظام (للتكرار أو الـ Seed)
    /// </summary>
    public static readonly IReadOnlyList<string> All = new[]
    {
        Admin,
        Doctor,
        Receptionist,
        Patient
    };

    /// <summary>
    /// الأدوار التي تتطلب حساباً إجبارياً (Staff)
    /// </summary>
    public static readonly IReadOnlyList<string> StaffRoles = new[]
    {
        Admin,
        Doctor,
        Receptionist
    };
}