namespace ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;

/// <summary>
/// النموذج الأساسي لكل قوالب البريد الإلكتروني في نظام ClinicOS
/// </summary>
public abstract class BaseEmailTemplateModel
{
    /// <summary>
    /// اسم العيادة (يظهر في الهيدر وتوقيع البريد)
    /// </summary>
    public string ClinicName { get; set; } = "ClinicOS";
    public string? ClinicAddress { get; set; }


    /// <summary>
    /// بريد الدعم الفني أو بريد التواصل للعيادة
    /// </summary>
    public string SupportEmail { get; set; } = string.Empty;

    /// <summary>
    /// رقم هاتف العيادة للتواصل (اختياري)
    /// </summary>
    public string? ClinicPhoneNumber { get; set; }

    /// <summary>
    /// عنوان العيادة الرئيسي (اختياري)
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// رابط موقع العيادة الإلكتروني
    /// </summary>
    public string? WebsiteUrl { get; set; }

    /// <summary>
    /// روابط منصات التواصل الاجتماعي (اختياري للفوتر)
    /// </summary>
    public string? FacebookUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? WhatsAppNumber { get; set; }

    /// <summary>
    /// سنة حقوق النشر الحالية (تولد ديناميكياً)
    /// </summary>
    public string Year => DateTime.UtcNow.Year.ToString();
}