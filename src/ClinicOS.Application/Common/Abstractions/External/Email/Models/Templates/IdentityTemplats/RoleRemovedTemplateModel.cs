using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;

namespace ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.IdentityTemplats;

/// <summary>
/// نموذج بريد إخطار إزالة دور
/// </summary>
public class RoleRemovedTemplateModel : BaseEmailTemplateModel
{
    public string UserName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? Reason { get; set; }
}