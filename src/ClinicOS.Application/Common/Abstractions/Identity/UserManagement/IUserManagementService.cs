using ClinicOS.Application.Features.Accounts.Shared;
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