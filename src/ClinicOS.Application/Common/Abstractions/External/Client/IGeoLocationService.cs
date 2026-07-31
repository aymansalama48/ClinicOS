using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Common.Abstractions.External.Client
{
    public interface IGeoLocationService
    {
        Task<string?> GetLocationAsync(
            string? ipAddress,
            CancellationToken cancellationToken);
    }
}
