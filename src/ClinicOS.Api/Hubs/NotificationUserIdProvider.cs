namespace ClinicOS.Api.Hubs;

using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

public class NotificationUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? connection.User?.FindFirst("sub")?.Value;
    }
}
