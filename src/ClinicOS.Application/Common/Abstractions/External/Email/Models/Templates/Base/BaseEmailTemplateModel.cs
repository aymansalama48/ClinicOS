namespace ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;

/// <summary>
/// النموذج الأساسي لكل قوالب البريد الإلكتروني في نظام ClinicOS
/// </summary>
public abstract class BaseEmailTemplateModel
{
    /// <summary>
    /// اسم العيادة (يظهر في توقيع البريد)
    /// </summary>
    public string ClinicName { get; set; } = "ClinicOS";

    /// <summary>
    /// رابط موقع العيادة (اختياري)
    /// </summary>
    public string? WebsiteUrl { get; set; }

    /// <summary>
    /// سنة حقوق النشر الحالية
    /// </summary>
    public string Year => DateTime.Now.Year.ToString();
}