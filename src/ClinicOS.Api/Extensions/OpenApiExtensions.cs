using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace ClinicOS.Api.Extensions;

public static class OpenApiExtensions
{
    /// <summary>
    /// تسجيل خدمات توليد مواصفات OpenAPI
    /// </summary>
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }

    /// <summary>
    /// تفعيل نقاط نهاية OpenAPI وواجهة Scalar في بيئة التطوير فقط
    /// </summary>
    public static WebApplication UseOpenApiDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();            // مسار ملف JSON: /openapi/v1.json
            app.MapScalarApiReference(); // مسار الواجهة: /scalar/v1
        }

        return app;
    }
}