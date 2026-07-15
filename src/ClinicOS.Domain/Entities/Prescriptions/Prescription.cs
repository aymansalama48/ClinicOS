using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Entities.MedicalRecords;

namespace ClinicOS.Domain.Entities.Prescriptions;

/// <summary>
/// الروشتة الطبية
/// </summary>
public class Prescription : AuditableEntity
{
    public Guid MedicalRecordId { get; set; }

    public DateTime IssueDate { get; set; }
    public string? Notes { get; set; }

    public virtual MedicalRecord MedicalRecord { get; set; } = null!;
    public virtual ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
}