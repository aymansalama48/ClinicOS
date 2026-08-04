using ClinicOS.Application.Common.Abstractions.Persistence.Data; // 👈 تأكد من إضافة الـ Namespace ده
using ClinicOS.Infrastructure.BackgroundJobs;
using ClinicOS.Infrastructure.Persistence.Data;
using ClinicOS.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicOS.Infrastructure.DependencyInjection;

public static partial class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. تسجيل الـ Interceptors الخاصة بـ Entity Framework
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<InsertOutboxMessagesInterceptor>();
        services.AddScoped<AuditLogInterceptor>();

        // 2. تسجيل الـ DbContext وربطه بـ SQL Server
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var softDeleteInterceptor = sp.GetRequiredService<SoftDeleteInterceptor>();
            var auditableInterceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
            var insertOutboxInterceptor = sp.GetRequiredService<InsertOutboxMessagesInterceptor>();
            var auditLogInterceptor = sp.GetRequiredService<AuditLogInterceptor>();

            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                   .AddInterceptors(
                       softDeleteInterceptor,
                       auditableInterceptor,
                       insertOutboxInterceptor,
                       auditLogInterceptor);
        });



        // 👇 تسجيل الـ Adapter ليربط الواجهة بالكلاس الجديد
        // Register both interfaces mapped to the same AppDbContext
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IReadDbContext>(provider => provider.GetRequiredService<AppDbContext>());


        // ✅ واكتب مكانه تسجيل الكلاس كـ Scoped:
        services.AddScoped<ProcessOutboxMessagesJob>();
        services.AddScoped<CleanupExpiredTokensJob>();
        services.AddScoped<AppointmentReminderJob>();
        services.AddScoped<RefreshDriveQuotaJob>();


        return services;
    }
}
