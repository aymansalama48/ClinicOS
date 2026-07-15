using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Infrastructure.Identity.Security;
using ClinicOS.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicOS.Infrastructure.DependencyInjection;

public static partial class DependencyInjection
{
    public static IServiceCollection AddOtpService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. ربط الإعدادات من appsettings.json بـ OtpOptions
        services.Configure<OtpOptions>(configuration.GetSection(OtpOptions.SectionName));

        // 2. تسجيل الخدمة IOtpService مع OtpService
        services.AddScoped<IOtpService, OtpService>();

        return services;
    }
}