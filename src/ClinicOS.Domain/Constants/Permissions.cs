using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ClinicOS.Domain.Constants;

/// <summary>
/// الثوابت الخاصة بالصلاحيات (Permissions) في النظام.
/// مقسمة بشكل هرمي (Hierarchical) لسهولة الاستخدام وقابلية التوسع.
/// </summary>
public static class Permissions
{
    public static class Dashboard
    {
        public const string Access = "Dashboard.Access";
    }

    public static class Specializations
    {
        public const string View = "Specializations.View";
        public const string Create = "Specializations.Create";
        public const string Update = "Specializations.Update";
        public const string Delete = "Specializations.Delete";
    }

    public static class Doctors
    {
        public const string View = "Doctors.View";
        public const string Create = "Doctors.Create";
        public const string Update = "Doctors.Update";
        public const string Delete = "Doctors.Delete";
        public const string ManageAvailability = "Doctors.ManageAvailability";
    }

    public static class Receptionists
    {
        public const string View = "Receptionists.View";
        public const string Create = "Receptionists.Create";
        public const string Update = "Receptionists.Update";
        public const string Delete = "Receptionists.Delete";
    }

    public static class Patients
    {
        public const string View = "Patients.View";
        public const string Create = "Patients.Create";
        public const string Update = "Patients.Update";
        public const string Delete = "Patients.Delete";
    }

    public static class Appointments
    {
        public const string View = "Appointments.View";
        public const string Create = "Appointments.Create";
        public const string Update = "Appointments.Update";
        public const string Cancel = "Appointments.Cancel";
        public const string Confirm = "Appointments.Confirm";
        public const string CheckIn = "Appointments.CheckIn";
    }

    public static class Queue
    {
        public const string View = "Queue.View";
        public const string Manage = "Queue.Manage";
    }

    public static class MedicalRecords
    {
        public const string View = "MedicalRecords.View";
        public const string Create = "MedicalRecords.Create";
        public const string Update = "MedicalRecords.Update";
        public const string Complete = "MedicalRecords.Complete";
    }

    public static class Prescriptions
    {
        public const string View = "Prescriptions.View";
        public const string Create = "Prescriptions.Create";
    }

    public static class Tests
    {
        public const string View = "Tests.View";
        public const string Order = "Tests.Order";
        public const string UploadResult = "Tests.UploadResult";
    }

    public static class Payments
    {
        public const string View = "Payments.View";
        public const string Create = "Payments.Create";
        public const string Refund = "Payments.Refund";
    }

    public static class Users
    {
        public const string View = "Users.View";
        public const string Manage = "Users.Manage"; // شاملة الإيقاف والتفعيل
        public const string AssignRoles = "Users.AssignRoles";
    }

    public static class Settings
    {
        public const string View = "Settings.View";
        public const string Manage = "Settings.Manage";
    }

    public static class Reports
    {
        public const string View = "Reports.View";
        public const string Export = "Reports.Export";
    }

    /// <summary>
    /// دالة سحرية تستخدم الـ Reflection لجلب جميع الصلاحيات المعرفة في هذا الكلاس.
    /// هذا يمنع خطأ نسيان إضافة صلاحية جديدة إلى قائمة الـ Seed.
    /// </summary>
    public static IReadOnlyList<string> GetAllPermissions()
    {
        var permissions = new List<string>();

        // نجيب كل الكلاسات الداخلية (Nested Classes)
        var nestedClasses = typeof(Permissions).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

        foreach (var nestedClass in nestedClasses)
        {
            // نجيب كل الثوابت (Constants) جوه الكلاس ده
            var constants = nestedClass.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(x => (string)x.GetRawConstantValue()!)
                .ToList();

            permissions.AddRange(constants);
        }

        return permissions.AsReadOnly();
    }
}