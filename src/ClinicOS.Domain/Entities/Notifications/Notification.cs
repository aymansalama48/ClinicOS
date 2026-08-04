namespace ClinicOS.Domain.Entities.Notifications;

using ClinicOS.Domain.Common.Entities;

public class Notification : AuditableEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // e.g., AppointmentConfirmed, Reminder
    public bool IsRead { get; set; }
    public string? ActionUrl { get; set; }
}
