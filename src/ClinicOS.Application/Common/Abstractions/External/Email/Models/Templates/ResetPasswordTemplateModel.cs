using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;

namespace ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates;

/// <summary>
/// نموذج بريد إعادة تعيين كلمة المرور
/// </summary>
public class ResetPasswordTemplateModel : BaseEmailTemplateModel
{
    public string UserName { get; set; } = string.Empty;
    public string ResetLink { get; set; } = string.Empty;
    public string? Email { get; set; }
}