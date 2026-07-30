using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Common.Abstractions.Core;

/// <summary>
/// خدمة مساعدة لجلب معرف التخصص المرتبط بالمستخدم (إن كان Doctor أو Receptionist)
/// </summary>
public interface ISpecializationService
{
    /// <summary>
    /// جلب معرف التخصص لمستخدم معين بناءً على userId
    /// </summary>
    Task<Guid?> GetSpecializationIdAsync(Guid userId, CancellationToken cancellationToken = default);
}