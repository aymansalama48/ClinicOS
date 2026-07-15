using ClinicOS.Application.Common.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ClinicOS.Application;

public static class DependencyInjection
{
    /// <summary>
    /// تسجيل كافة خدمات طبقة الـ Application (MediatR, FluentValidation, AutoMapper/Mapster)
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // 1. تسجيل MediatR للتعامل مع الـ Commands والـ Queries والـ Event Handlers
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            // 1. Logging أولاً لتتبع بداية ونهاية كل Request
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));

            // 2. Performance لمراقبة الوقت الكلي
            cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));

            // 3. Authorization للتحقق من الصلاحيات قبل أي شيء آخر
            cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));

            // 4. Validation للتأكد من صحة المدخلات
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));

            // 5. Transaction أخيراً لإدارة المعاملة أثناء تنفيذ الـ Handler
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        // 2. فحص الـ Assembly وتنسيق كافة كلاسات الـ FluentValidation تلقائياً
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}