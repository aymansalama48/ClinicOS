using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Common.Abstractions.Web
{
    public interface IClientContext
    {
        string? IpAddress { get; }

        string? UserAgent { get; }
    }
}
