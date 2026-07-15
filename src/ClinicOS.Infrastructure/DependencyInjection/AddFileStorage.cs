using ClinicOS.Application.Common.Abstractions.External.FileStorage;
using ClinicOS.Infrastructure.External;
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
        /// <summary>
        /// تسجيل خدمات تخزين الملفات.
        /// </summary>
        public static IServiceCollection AddFileStorage(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<FileStorageOptions>(
                configuration.GetSection(FileStorageOptions.SectionName));

            services.AddScoped<IFileStorage, LocalFileStorage>();

            return services;
        }
    }
}
