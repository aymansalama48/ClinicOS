using System.IO;
using ClinicOS.Api.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

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

        // 2. معالجة الاستثناءات وتسجيل الـ Requests عبر Serilog بالخيارات المخصصة
        app.UseExceptionHandler();
        app.UseSerilogLogging(); // 👈 استبدال app.UseSerilogRequestLogging() هنا

        // 3. التوجيه الآمن والـ CORS
        app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");

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

        // 5. توثيق OpenAPI/Scalar
        app.UseOpenApiDocumentation();

        // 6. التوثيق والصلاحيات
        app.UseAuthentication();
        app.UseAuthorization();

        // 7. ربط الـ Controllers
        app.MapControllers();

        return app;
    }
}