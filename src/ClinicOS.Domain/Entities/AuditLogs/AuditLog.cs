using ClinicOS.Domain.Common.Entities;

namespace ClinicOS.Domain.Entities;

/// <summary>
/// سجل التدقيق المركزي
/// </summary>
public class AuditLog : AuditableEntity
{
    public Guid? UserId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
    public string? AdditionalInfo { get; set; }
    public string? IpAddress { get; set; }
}