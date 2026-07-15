using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;

namespace ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates;

/// <summary>
/// نموذج بريد إخطار تغيير كلمة المرور
/// </summary>
public class PasswordChangedTemplateModel : BaseEmailTemplateModel
{
    public string UserName { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; } = DateTime.Now;
    public string? SecurityNotice { get; set; }
}