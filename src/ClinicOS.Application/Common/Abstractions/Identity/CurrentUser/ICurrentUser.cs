namespace ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;

/// <summary>
/// يقرأ بيانات المستخدم الحالي من الـ Claims الموجودة في التوكن (JWT)
/// ممكن يمثل Staff (Admin/Doctor/Receptionist) أو Patient حسب نوع التوكن
/// </summary>
public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    // === مشتركة بين المريض والـ Staff ===
    Guid? UserId { get; }
    string? FullName { get; }

    // === خاصة بالمريض فقط (لو PatientId مش null، يبقى التوكن ده توكن مريض) ===
    Guid? PatientId { get; }
    string? PhoneNumber { get; }

    // === خاصة بالـ Staff فقط ===
    string? Email { get; }
    Guid? SpecializationId { get; }   // موجودة للـ Doctor والـ Receptionist بس، null للـ Admin

    string? Role { get; }
    IReadOnlyList<string> Roles { get; }
    bool IsInRole(string role);

    bool HasPermission(string permission);
    IEnumerable<string> GetPermissions();
}