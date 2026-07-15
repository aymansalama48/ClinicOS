using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Enums;

namespace ClinicOS.Domain.Entities.MedicalRecords;

/// <summary>
/// طلب تحليل مخبري
/// </summary>
public class Test : AuditableEntity
{
    public Guid MedicalRecordId { get; set; }

    public string TestName { get; set; } = string.Empty;
    public DateTime OrderedAt { get; set; }
    public TestStatus Status { get; set; } = TestStatus.Ordered;

    public string? Result { get; set; }
    public string? ResultFileUrl { get; set; }
    public DateTime? ResultUploadedAt { get; set; }
    public Guid? ResultUploadedBy { get; set; }

    public virtual MedicalRecord MedicalRecord { get; set; } = null!;
}