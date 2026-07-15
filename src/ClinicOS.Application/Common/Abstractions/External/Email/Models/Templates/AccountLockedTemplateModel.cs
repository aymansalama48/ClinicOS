using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;

namespace ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates;

/// <summary>
/// نموذج بريد إخطار قفل الحساب
/// </summary>
public class AccountLockedTemplateModel : BaseEmailTemplateModel
{
    /// <summary>
    /// اسم المستخدم
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// سبب القفل
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// رابط فتح القفل أو الدعم الفني
    /// </summary>
    public string? UnlockUrl { get; set; }
}