using ClinicOS.Application.Common.Abstractions.Persistence; // 👈 تأكد من إضافة الـ Namespace ده
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

        // 2. تسجيل الـ DbContext وربطه بـ SQL Server
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var softDeleteInterceptor = sp.GetRequiredService<SoftDeleteInterceptor>();
            var auditableInterceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
            var insertOutboxInterceptor = sp.GetRequiredService<InsertOutboxMessagesInterceptor>();

            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                   .AddInterceptors(
                       softDeleteInterceptor,
                       auditableInterceptor,
                       insertOutboxInterceptor);
        });

        // 👈 2.1 ربط الـ Interface بالـ DbContext الفعلي (هذا السطر المطلوب)
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        // 3. تسجيل الـ Outbox Process Job
        //services.AddHostedService<ProcessOutboxMessagesJob>();
        
        
        // ✅ واكتب مكانه تسجيل الكلاس كـ Scoped:
        services.AddScoped<ProcessOutboxMessagesJob>();


        return services;
    }
}