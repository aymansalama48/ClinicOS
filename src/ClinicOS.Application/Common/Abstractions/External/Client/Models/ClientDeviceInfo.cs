using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Common.Abstractions.External.Client.Models
{
    public sealed record ClientDeviceInfo(
        string Browser,
        string OperatingSystem,
        string DeviceType);
}
