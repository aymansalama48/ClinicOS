using ClinicOS.Application.Common.Abstractions.External.Client.Models;


namespace ClinicOS.Application.Common.Abstractions.External.Client
{
    public interface IUserAgentParser
    {
        ClientDeviceInfo Parse(string? userAgent);
    }
}
