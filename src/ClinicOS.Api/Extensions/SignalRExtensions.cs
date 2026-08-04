namespace ClinicOS.Api.Extensions;

using ClinicOS.Api.Hubs;
using ClinicOS.Application.Common.Abstractions.Notifications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;

public static class SignalRExtensions
{
    public static IServiceCollection AddApplicationSignalR(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddSingleton<IUserIdProvider, NotificationUserIdProvider>();
        services.AddTransient<INotificationSender, SignalRNotificationSender>();

        // Configure JWT Bearer to support access_token query param for WebSockets
        services.ConfigureOptions<ConfigureJwtBearerOptions>();

        return services;
    }
}

public class ConfigureJwtBearerOptions : Microsoft.Extensions.Options.IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }

    public void Configure(JwtBearerOptions options)
    {
        var originalOnMessageReceived = options.Events?.OnMessageReceived;
        
        if (options.Events == null)
        {
            options.Events = new JwtBearerEvents();
        }

        options.Events.OnMessageReceived = async context =>
        {
            if (originalOnMessageReceived != null)
            {
                await originalOnMessageReceived(context);
            }

            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
        };
    }
}
