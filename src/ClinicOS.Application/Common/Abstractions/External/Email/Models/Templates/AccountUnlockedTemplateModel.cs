using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;

namespace ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates;

/// <summary>
/// نموذج بريد إخطار فتح القفل
/// </summary>
public class AccountUnlockedTemplateModel : BaseEmailTemplateModel
{
    public string UserName { get; set; } = string.Empty;
    public string? LoginUrl { get; set; }
}