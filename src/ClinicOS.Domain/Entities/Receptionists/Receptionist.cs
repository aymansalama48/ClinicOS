using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Entities.Specializations;

namespace ClinicOS.Domain.Entities.Receptionists;

/// <summary>
/// موظف الاستقبال
/// </summary>
public class Receptionist : SoftDeleteEntity
{
    public Guid SpecializationId { get; set; }
    public Guid ApplicationUserId { get; set; } // حساب Identity إجباري
    public bool IsActive { get; set; } = true;

    public virtual Specialization Specialization { get; set; } = null!;
}