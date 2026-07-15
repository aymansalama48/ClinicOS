using ClinicOS.Application.Common.Abstractions.External.Email;
using ClinicOS.Infrastructure.External.Email;
using ClinicOS.Infrastructure.Options;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Infrastructure.DependencyInjection
{
    public static partial class DependencyInjection
    {
        /// <summary>
        /// تسجيل خدمات البريد الإلكتروني (Email Services).
        /// </summary>
        public static IServiceCollection AddMail(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // إعدادات الـ Mail
            services.Configure<MailOptions>(configuration.GetSection(MailOptions.SectionName));

            // تسجيل محرك القوالب وخدمة الإرسال
            services.AddScoped<EmailTemplateEngine>();
            services.AddScoped<IEmailSender, SmtpEmailSender>();

            return services;
        }
    }
}
