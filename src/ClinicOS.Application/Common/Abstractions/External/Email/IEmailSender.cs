using ClinicOS.Application.Common.Abstractions.External.Email.Models;

namespace ClinicOS.Application.Common.Abstractions.External.Email;

/// <summary>
/// واجهة إرسال البريد الأساسية (Pure Infrastructure Service)
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// إرسال بريد إلكتروني مخصص
    /// </summary>
    Task SendEmailAsync(EmailRequest request);
}