using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Enums;

namespace ClinicOS.Domain.Entities.MedicalRecords;

/// <summary>
/// طلب تحليل مخبري
/// </summary>
public class Test : AuditableEntity
{
    public Guid MedicalRecordId { get; private set; }

    public string TestName { get; private set; } = string.Empty;
    public DateTime OrderedAt { get; private set; }
    public TestStatus Status { get; private set; } = TestStatus.Ordered;

    public string? Result { get; private set; }
    public string? ResultFileUrl { get; private set; }
    public DateTime? ResultUploadedAt { get; private set; }
    public Guid? ResultUploadedBy { get; private set; }

    public virtual MedicalRecord MedicalRecord { get; private set; } = null!;

    protected Test() { }

    public static Test Create(string testName, string? result = null)
    {
        return new Test
        {
            Id = Guid.NewGuid(),
            TestName = testName,
            OrderedAt = DateTime.UtcNow,
            Status = string.IsNullOrEmpty(result) ? TestStatus.Ordered : TestStatus.Completed,
            Result = result
        };
    }
}