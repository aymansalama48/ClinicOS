using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;

namespace ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.IdentityTemplats;

/// <summary>
/// نموذج بريد الترحيب بالمستخدم الجديد
/// </summary>
public class WelcomeTemplateModel : BaseEmailTemplateModel
{
    public string UserName { get; set; } = string.Empty;
    public string? LoginUrl { get; set; }
}