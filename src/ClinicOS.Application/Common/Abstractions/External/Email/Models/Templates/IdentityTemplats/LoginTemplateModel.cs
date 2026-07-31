using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.IdentityTemplats
{
    public sealed class LoginTemplateModel: BaseEmailTemplateModel
    {
        public string UserName { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;   // مثال: Chrome on Windows 11
        public string Location { get; set; } = string.Empty; // مثال: Cairo, Egypt
        public string UserAgent { get; set; } = string.Empty;
    }
}
