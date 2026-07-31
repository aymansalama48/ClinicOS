using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;

namespace ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.IdentityTemplats;

/// <summary>
/// نموذج بريد إعادة تعيين كلمة المرور
/// </summary>
public class ResetPasswordTemplateModel : BaseEmailTemplateModel
{
    /// <summary>
    /// اسم المستخدم الموجه له البريد
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// رابط إعادة تعيين كلمة المرور الحاوي على التوكن
    /// </summary>
    public string ResetLink { get; set; } = string.Empty;

    /// <summary>
    /// البريد الإلكتروني الخاص بحساب المستخدم
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// مدة صلاحية الرابط بالدقائق (مثلاً: 15 دقيقة)
    /// </summary>
    public int ExpirationInMinutes { get; set; } = 15;

    public string IpAddress { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}