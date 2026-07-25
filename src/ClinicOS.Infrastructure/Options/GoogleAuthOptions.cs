using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Infrastructure.Options
{
    public sealed class GoogleAuthOptions
    {
        public const string SectionName = "GoogleAuth";

        public string ClientId { get; set; } = string.Empty;
    }
}
