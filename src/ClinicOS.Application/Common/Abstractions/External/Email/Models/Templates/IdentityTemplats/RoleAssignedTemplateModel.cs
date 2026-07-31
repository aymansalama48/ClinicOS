using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;

namespace ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.IdentityTemplats;

/// <summary>
/// نموذج بريد إخطار تعيين دور جديد
/// </summary>
public class RoleAssignedTemplateModel : BaseEmailTemplateModel
{
    public string UserName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? DashboardUrl { get; set; }
}