using ClinicOS.Application.Common.Abstractions.External.Routing;
using ClinicOS.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace ClinicOS.Infrastructure.External.Routing;

public sealed class ApplicationUrlService : IApplicationUrlService
{
    private readonly BaseUrlOptions _options;

    public ApplicationUrlService(
        IOptions<BaseUrlOptions> options)
    {
        _options = options.Value;
    }

    public string GeneratePasswordResetUrl(
        string email,
        string token)
    {
        return string.Concat(
            _options.Frontend.TrimEnd('/'),
            "/reset-password",
            "?email=",
            Uri.EscapeDataString(email),
            "&token=",
            Uri.EscapeDataString(token));
    }
}