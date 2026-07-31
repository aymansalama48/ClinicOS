using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.External.Client;
using ClinicOS.Application.Common.Abstractions.External.Routing;
using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Identity.Invitations;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Application.Common.Abstractions.Web;
using ClinicOS.Infrastructure.Core;
using ClinicOS.Infrastructure.External.Client;
using ClinicOS.Infrastructure.External.Routing;
using ClinicOS.Infrastructure.Identity.CurrentUser;
using ClinicOS.Infrastructure.Identity.Invitations;
using ClinicOS.Infrastructure.Notifications;
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
               .AddCaching()                         // تسجيل خدمات الـ Caching
               .AddPersistence(configuration)        // قاعدة البيانات
               .AddHangfireJobs(configuration)       // تسجيل خدمات Hangfire
               .AddIdentityServices()               // 👈 1. تسجيل Identity أولاً (لتجهيز الجداول و الـ Stores)
               .AddJwtAuthentication(configuration)  // 👈 2. تسجيل JWT بعدها فوراً (ليكتاب فوق الـ Default Schemes ويجعلها JWT)
               .AddExternalAuth(configuration)       // تسجيل المصادقة الخارجية (Google Auth)
               .AddMail(configuration)               // البريد الإلكتروني
               .AddFileStorage(configuration)        // تخزين الملفات
               .AddBaseUrl(configuration)            // الروابط الأساسية
               .AddOtpService(configuration);        // إضافة OTP Service

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

        // تسجيل خدمة جلب معرف التخصص المرتبط بالمستخدم (إن كان Doctor أو Receptionist)
        services.AddScoped<ISpecializationService, SpecializationService>();

        // تسجيل خدمة توليد الروابط الأساسية للتطبيق
        services.AddScoped<IApplicationUrlService, ApplicationUrlService>();



        // تسجيل خدمات العميل
        services.AddScoped<IClientContext, HttpClientContext>();
        services.AddSingleton<IUserAgentParser, UserAgentParser>();

        // تسجيل GeoLocationService مع HttpClient مخصص وتحديد Timeout 3 ثواني فقط
        services.AddHttpClient<IGeoLocationService, GeoLocationService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(3);// مهلة 3 ثوانٍ لحماية الـ Hangfire Worker من التعليق
        });

        // خدمات الإشعارات الوظيفية
        services.AddTransient<IIdentityNotificationService, IdentityNotificationService>();
        services.AddTransient<IAppointmentNotificationService, AppointmentNotificationService>();

        services.AddScoped<IInvitationService, InvitationService>();

        return services;
    }
}