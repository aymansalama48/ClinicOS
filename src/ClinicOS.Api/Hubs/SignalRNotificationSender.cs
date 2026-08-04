namespace ClinicOS.Api.Hubs;

using ClinicOS.Application.Common.Abstractions.Notifications;
using Microsoft.AspNetCore.SignalR;

public sealed class SignalRNotificationSender(IHubContext<NotificationHub> hubContext) : INotificationSender
{
    public async Task SendToUserAsync(Guid userId, string eventName, object payload, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.User(userId.ToString()).SendAsync(eventName, payload, cancellationToken);
    }

    public async Task SendToGroupAsync(string group, string eventName, object payload, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.Group(group).SendAsync(eventName, payload, cancellationToken);
    }

    public async Task SendToAllAsync(string eventName, object payload, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.All.SendAsync(eventName, payload, cancellationToken);
    }
}
