using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Infrastructure.Core;
using ClinicOS.Infrastructure.Identity.CurrentUser;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicOS.Infrastructure.DependencyInjection;

/// <summary>
/// الكلاس المسؤول عن تجميع وتسجيل كافة خدمات طبقة الـ Infrastructure في حاوية الـ DI
/// </summary>
public static partial class DependencyInjection
{
    /// <summary>
    /// Extension method لتسجيل كافة مكونات الـ Infrastructure دفعة واحدة
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
               .AddCoreServices()                    // الخدمات الأساسية
               .AddCaching()                         // ✅ تسجيل خدمات الـ Caching
               .AddPersistence(configuration)        // قاعدة البيانات
               .AddJwtAuthentication(configuration)  // التوثيق
               .AddExternalAuth(configuration)       // تسجيل المصادقة الخارجية (Google Auth)
               .AddMail(configuration)               // البريد الإلكتروني
               .AddFileStorage(configuration)        // تخزين الملفات
               .AddBaseUrl(configuration)            // الروابط الأساسية
               .AddOtpService(configuration)         // إضافة OTP Service
               .AddIdentityServices();               // إضافة خدمات Identity

        return services;
    }

    /// <summary>
    /// تسجيل خدمات السياق والبيانات الأساسية للنظام (Core Context Services)
    /// </summary>
    private static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // تمكين قراءة الـ HttpContext الحالي من أي خدمة داخل التطبيق
        services.AddHttpContextAccessor();

        // تسجيل خدمة الوقت الموحدة للنظام
        services.AddTransient<IDateTime, DateTimeProvider>();

        // تسجيل خدمة الوصول لبيانات وهوية المستخدم الحالي المسجل بالطلب
        services.AddScoped<ICurrentUser, CurrentUserService>();

        // تسجيل خدمة تتبع معرف الطلب الفريد (Correlation ID)
        services.AddScoped<ICorrelationContext, CorrelationContext>();

        return services;
    }
}