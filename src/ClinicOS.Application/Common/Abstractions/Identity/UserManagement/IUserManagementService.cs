using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Common.Abstractions.Identity.UserManagement;

/// <summary>
/// إدارة سجل ApplicationUser الأساسي — تخدم الـ Staff والمريض صاحب الحساب الدائم على حدٍ سواء
/// (كل الـ Methods بتشتغل بالـ userId بس، من غير أي منطق خاص بدور معين)
/// </summary>
public interface IUserManagementService
{
    Task<Result<UserDto>> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<Result> EnsureUserExistsAsync(
        Guid userId,
        CancellationToken cancellationToken);

    /// تعطيل تسجيل الدخول بس — لو المستخدم مريض، حجوزاته وتاريخه الطبي مش بيتأثروا
    Task<Result> DeactivateUserAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<Result> ActivateUserAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<Result> UpdateProfileAsync(
        Guid userId,
        string fullName,
        string phoneNumber,
        CancellationToken cancellationToken);

    Task<Result> UpdateProfilePictureAsync(
        Guid userId,
        string avatarUrl,
        CancellationToken cancellationToken);
}
/// <summary>
/// بيانات المستخدم الأساسية (تستخدم للـ Staff والمريض صاحب الحساب الدائم)
/// </summary>
public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string? MiddleName { get; init; }
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? AvatarUrl { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }

    // ✅ إضافة Roles لأنها مستخدمة في UserManagementService
    public List<string> Roles { get; init; } = new();
}