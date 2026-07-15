namespace ClinicOS.Domain.Constants;

/// <summary>
/// الثوابت الخاصة بالصلاحيات (Permissions) في النظام
/// مصممة حسب الوحدات الوظيفية (Modules) لتسهيل الإدارة والبحث
/// </summary>
public static class Permissions
{
    // ====================================================
    // 1. وحدة لوحة التحكم (Dashboard)
    // ====================================================
    public const string DashboardAccess = "Dashboard.Access";

    // ====================================================
    // 2. وحدة المواعيد (Appointments)
    // ====================================================
    public const string AppointmentsView = "Appointments.View";
    public const string AppointmentsCreate = "Appointments.Create";
    public const string AppointmentsUpdate = "Appointments.Update";
    public const string AppointmentsCancel = "Appointments.Cancel";
    public const string AppointmentsConfirm = "Appointments.Confirm";   // تأكيد OTP
    public const string AppointmentsCheckIn = "Appointments.CheckIn";   // دخول الطابور

    // ====================================================
    // 3. وحدة الطابور (Queue)
    // ====================================================
    public const string QueueView = "Queue.View";
    public const string QueueManage = "Queue.Manage";   // استدعاء المريض التالي، إعادة ترتيب

    // ====================================================
    // 4. وحدة السجلات الطبية (Medical Records)
    // ====================================================
    public const string MedicalRecordsView = "MedicalRecords.View";
    public const string MedicalRecordsCreate = "MedicalRecords.Create";
    public const string MedicalRecordsUpdate = "MedicalRecords.Update";
    public const string MedicalRecordsComplete = "MedicalRecords.Complete"; // إنهاء الكشف

    // ====================================================
    // 5. وحدة الروشتات والتحاليل (Prescriptions & Tests)
    // ====================================================
    public const string PrescriptionsCreate = "Prescriptions.Create";
    public const string PrescriptionsView = "Prescriptions.View";
    public const string TestsOrder = "Tests.Order";
    public const string TestsUploadResult = "Tests.UploadResult";
    public const string TestsView = "Tests.View";

    // ====================================================
    // 6. وحدة المدفوعات (Payments)
    // ====================================================
    public const string PaymentsView = "Payments.View";
    public const string PaymentsCreate = "Payments.Create";
    public const string PaymentsRefund = "Payments.Refund";

    // ====================================================
    // 7. وحدة إدارة المستخدمين (User Management)
    // ====================================================
    public const string UsersView = "Users.View";
    public const string UsersCreate = "Users.Create";
    public const string UsersUpdate = "Users.Update";
    public const string UsersDeactivate = "Users.Deactivate";
    public const string UsersActivate = "Users.Activate";
    public const string UsersAssignRoles = "Users.AssignRoles";

    // ====================================================
    // 8. وحدة الإعدادات والتكوين (Settings & Configuration)
    // ====================================================
    public const string SettingsView = "Settings.View";
    public const string SettingsManage = "Settings.Manage";
    public const string SpecializationsManage = "Specializations.Manage";

    // ====================================================
    // 9. وحدة التقارير (Reports)
    // ====================================================
    public const string ReportsView = "Reports.View";
    public const string ReportsExport = "Reports.Export";

    // ====================================================
    // 10. وحدة المرضى (Patients) - للموظفين فقط
    // ====================================================
    public const string PatientsView = "Patients.View";
    public const string PatientsCreate = "Patients.Create";
    public const string PatientsUpdate = "Patients.Update";

    // ====================================================
    // 11. وحدة الأطباء والتخصصات (Doctors & Specializations) - للإدارة
    // ====================================================
    public const string DoctorsManage = "Doctors.Manage";
    public const string DoctorsView = "Doctors.View";
    public const string SpecializationsView = "Specializations.View";

    // ====================================================
    // قائمة بجميع الصلاحيات (للتكرار أو الـ Seed)
    // ====================================================
    public static readonly IReadOnlyList<string> All = new[]
    {
        DashboardAccess,

        AppointmentsView,
        AppointmentsCreate,
        AppointmentsUpdate,
        AppointmentsCancel,
        AppointmentsConfirm,
        AppointmentsCheckIn,

        QueueView,
        QueueManage,

        MedicalRecordsView,
        MedicalRecordsCreate,
        MedicalRecordsUpdate,
        MedicalRecordsComplete,

        PrescriptionsCreate,
        PrescriptionsView,
        TestsOrder,
        TestsUploadResult,
        TestsView,

        PaymentsView,
        PaymentsCreate,
        PaymentsRefund,

        UsersView,
        UsersCreate,
        UsersUpdate,
        UsersDeactivate,
        UsersActivate,
        UsersAssignRoles,

        SettingsView,
        SettingsManage,
        SpecializationsManage,

        ReportsView,
        ReportsExport,

        PatientsView,
        PatientsCreate,
        PatientsUpdate,

        DoctorsManage,
        DoctorsView,
        SpecializationsView
    };
}