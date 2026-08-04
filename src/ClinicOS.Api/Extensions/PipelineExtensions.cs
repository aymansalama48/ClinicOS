using ClinicOS.Api.Hubs;
using ClinicOS.Api.Middlewares;
using ClinicOS.Infrastructure.BackgroundJobs;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace ClinicOS.Api.Extensions;

public static class PipelineExtensions
{
    /// <summary>
    /// تطبيق خط سير الطلبات (Request Pipeline) بترتيبه الصحيح
    /// </summary>
    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {





        // 1. تشغيل Correlation ID في أسرع نقطة دخول للطلب لتتبع الـ Requests
        app.UseMiddleware<CorrelationIdMiddleware>();

        // 1.1 Security Headers & Rate Limiting
        app.UseMiddleware<SecurityHeadersMiddleware>();
        app.UseMiddleware<RateLimitIdentityMiddleware>();
        app.UseRateLimiter();

        // 2. معالجة الاستثناءات وتسجيل الـ Requests عبر Serilog بالخيارات المخصصة
        app.UseExceptionHandler();
        app.UseSerilogLogging(); // 👈 استبدال app.UseSerilogRequestLogging() هنا

        // 3. التوجيه الآمن والـ CORS
        app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");
        // 👇 السطر ده هو الحل (لازم تضيفه هنا عشان الواجهة تفتح)
        app.UseStaticFiles();
        // 4. إدارة الملفات المرفوعة المباشرة (Static Files)
        var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "uploads");
        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(uploadsPath),
            RequestPath = "/uploads"
        });

        // 6. التوثيق والصلاحيات
        app.UseAuthentication();
        app.UseAuthorization();


        // 7. تفعيل شاشة Hangfire للمراقبة
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new HangfireCustomAuthorizationFilter() }
        });
        RecurringJob.AddOrUpdate<ProcessOutboxMessagesJob>(
            "process-outbox-messages",
            job => job.ProcessAsync(),
            "*/5 * * * * *"); // Cron Expression للتكرار كل 5 ثوانٍ

        RecurringJob.AddOrUpdate<CleanupExpiredTokensJob>(
            "cleanup-expired-tokens",
            job => job.ProcessAsync(CancellationToken.None),
            Cron.Daily);

        RecurringJob.AddOrUpdate<AppointmentReminderJob>(
            "appointment-reminders",
            job => job.ProcessAsync(CancellationToken.None),
            Cron.Daily(18)); // 6:00 PM every day

        RecurringJob.AddOrUpdate<RefreshDriveQuotaJob>(
            "refresh-drive-quota",
            job => job.ProcessAsync(CancellationToken.None),
            Cron.Weekly);


        // 8. ربط الـ Controllers
        app.MapControllers();

        // 9. ربط الـ SignalR Hubs
        app.MapHub<NotificationHub>(NotificationHub.Route);

        // 10. توثيق OpenAPI/Scalar
        app.UseOpenApiDocumentation();

        return app;
    }
}