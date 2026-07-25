using ClinicOS.Application.Common.Abstractions.Identity.Providers;
using ClinicOS.Infrastructure.Identity.Providers;
using ClinicOS.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Infrastructure.DependencyInjection
{
    public static partial class DependencyInjection
    {
        private static IServiceCollection AddExternalAuth(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<GoogleAuthOptions>(configuration.GetSection(GoogleAuthOptions.SectionName));


            services.AddScoped<IExternalAuthProvider, GoogleAuthProvider>();


            return services;
        }
    }
}
