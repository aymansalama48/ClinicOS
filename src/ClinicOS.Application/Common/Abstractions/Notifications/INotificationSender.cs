namespace ClinicOS.Application.Common.Abstractions.Notifications;

public interface INotificationSender
{
    Task SendToUserAsync(Guid userId, string eventName, object payload, CancellationToken cancellationToken = default);
    Task SendToGroupAsync(string group, string eventName, object payload, CancellationToken cancellationToken = default);
    Task SendToAllAsync(string eventName, object payload, CancellationToken cancellationToken = default);
}
