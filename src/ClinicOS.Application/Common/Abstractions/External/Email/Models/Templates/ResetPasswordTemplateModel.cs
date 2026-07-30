using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;

namespace ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates;

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

    /// <summary>
    /// عنوان الـ IP الذي أرسل الطلب (لأغراض الأمان)
    /// </summary>
    public string? RequestIpAddress { get; set; }

    /// <summary>
    /// متصفح أو نظام التشغيل الذي طلب الإعادة
    /// </summary>
    public string? UserAgent { get; set; }
}