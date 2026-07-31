using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Entities.Appointments;
using ClinicOS.Domain.Enums;

namespace ClinicOS.Domain.Entities.Patients
{
    /// <summary>
    /// المريض (يمكن أن يكون بدون حساب مستخدم)
    /// </summary>
    public class Patient : SoftDeleteEntity
    {
        // البيانات الشخصية الأساسية
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// يعيد الاسم الكامل مع معالجة حقل الاسم الأوسط الاختياري بدون مسافات إضافية.
        /// </summary>
        public string FullName => string.IsNullOrWhiteSpace(MiddleName)
            ? $"{FirstName} {LastName}".Trim()
            : $"{FirstName} {MiddleName} {LastName}".Trim();
        public string PhoneNumber { get; set; } = string.Empty;

        // بيانات اختيارية - ممكن تتعبى بعدين أو تفضل فاضية
        public DateOnly? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public BloodType? BloodType { get; set; }
        public string? EmergencyContact { get; set; }

        // فاضي (null) لو المريض بيحجز بالتليفون بس من غير حساب دائم
        // بتتحط قيمته لما المريض يقرر يعمل حساب بإيميل وباسورد
        public Guid? ApplicationUserId { get; set; }

        // خانة سريعة تقول هل المريض ده عنده حساب دائم مربوط ولا لسه بيحجز بالتليفون بس
        public bool IsAccountLinked => ApplicationUserId.HasValue;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}