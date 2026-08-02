using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Entities.Specializations;

namespace ClinicOS.Domain.Entities.Receptionists;

/// <summary>
/// موظف الاستقبال
/// </summary>
public class Receptionist : SoftDeleteEntity
{

    public Guid SpecializationId { get; set; }
    public Guid ApplicationUserId { get; set; }
    public bool IsActive { get; set; }

    public virtual Specialization Specialization { get; set; } = null!;


    protected Receptionist() { }

    private Receptionist(Guid applicationUserId, Guid specializationId)
    {
        ApplicationUserId = applicationUserId;
        SpecializationId = specializationId;
        IsActive = true;
    }

    public static Receptionist Create(Guid applicationUserId, Guid specializationId)
    {
        return new Receptionist(applicationUserId, specializationId);
    }

    public void UpdateProfile(Guid specializationId)
    {
        SpecializationId = specializationId;
    }
}