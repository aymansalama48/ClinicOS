namespace ClinicOS.Domain.Entities.Emails;
using ClinicOS.Domain.Common.Entities;

public class EmailLog : BaseEntity
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsSent { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime SentAtUtc { get; set; }
}
