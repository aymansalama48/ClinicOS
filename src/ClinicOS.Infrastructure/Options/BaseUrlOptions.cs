using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Infrastructure.Options
{
    public sealed class BaseUrlOptions
    {
        public const string SectionName = "BaseUrl";

        public string BaseUrl { get; set; } = string.Empty;
    }
}
