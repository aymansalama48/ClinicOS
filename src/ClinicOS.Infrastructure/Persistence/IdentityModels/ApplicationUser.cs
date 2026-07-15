using ClinicOS.Domain.Doctors;
using ClinicOS.Domain.Entities.Doctors;
using ClinicOS.Domain.Entities.Patients;
using ClinicOS.Domain.Entities.Receptionists;
using Microsoft.AspNetCore.Identity;

namespace ClinicOS.Infrastructure.Persistence.IdentityModels;

/// <summary>
/// يمثل حساب المستخدم الفعلي لتسجيل الدخول في نظام ClinicOS.
/// - إجباري للموظفين (Doctor, Receptionist, Admin)
/// - اختياري للمرضى (Patient)
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
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

    /// <summary>
    /// رابط صورة البروفايل الشخصية (اختياري).
    /// </summary>
    public string? AvatarUrl { get; set; }

    // حالة الحساب
    public bool IsActive { get; set; } = true;

    // بيانات التتبع وتاريخ الدخول
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    // Navigation Properties - العلاقات مع باقي الكيانات
    public Doctor? Doctor { get; set; }
    public Receptionist? Receptionist { get; set; }
    public Patient? Patient { get; set; } 
}