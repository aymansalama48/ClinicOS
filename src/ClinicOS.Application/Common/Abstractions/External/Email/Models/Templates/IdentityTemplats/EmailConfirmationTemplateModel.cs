using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;

namespace ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.IdentityTemplats;

/// <summary>
/// نموذج بريد تأكيد البريد الإلكتروني
/// </summary>
public class EmailConfirmationTemplateModel : BaseEmailTemplateModel
{
    public string UserName { get; set; } = string.Empty;
    public string ConfirmationLink { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}