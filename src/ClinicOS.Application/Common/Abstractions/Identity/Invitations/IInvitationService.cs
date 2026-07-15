using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Common.Abstractions.Identity.Invitations;

public interface IInvitationService
{
    /// <summary>
    /// إنشاء دعوة لموظف جديد (Doctor / Receptionist) وتوليد رابط آمن
    /// </summary>
    Task<Result<string>> SendStaffInvitationAsync(
        string email,
        string role,
        Guid? specializationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// التحقق من صحة توكن الدعوة قبل عرض صفحة إنشاء الحساب للموظف
    /// </summary>
    Task<Result<InvitationDetailsDto>> ValidateInvitationTokenAsync(
        string invitationToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// إكمال الموظف لبياناته وإنشاء حساب ApplicationUser مرتبط برابط الدعوة
    /// </summary>
    Task<Result<bool>> AcceptInvitationAndCreateAccountAsync(
        string invitationToken,
        string fullName,
        string password,
        CancellationToken cancellationToken = default);
}

public record InvitationDetailsDto(
    string Email,
    string Role,
    Guid? SpecializationId,
    bool IsValid
);