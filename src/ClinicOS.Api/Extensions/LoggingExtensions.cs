using Microsoft.AspNetCore.Builder;
using Serilog;

namespace ClinicOS.Api.Extensions;

public static class LoggingExtensions
{
    /// <summary>
    /// تهيئة Serilog وربطه بـ Host الخاص بالتطبيق
    /// </summary>
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }
}