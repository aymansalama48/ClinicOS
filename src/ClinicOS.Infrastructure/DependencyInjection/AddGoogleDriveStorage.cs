namespace ClinicOS.Infrastructure.DependencyInjection;

using ClinicOS.Application.Common.Abstractions.External.FileStorage;
using ClinicOS.Infrastructure.External.Storage;
using ClinicOS.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static partial class DependencyInjection
{
    public static IServiceCollection AddGoogleDriveStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<GoogleDriveOptions>(
            configuration.GetSection(GoogleDriveOptions.SectionName));

        services.AddScoped<IFileStorage, GoogleDriveContentStorage>();

        return services;
    }
}
