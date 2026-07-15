using ClinicOS.Application.Common.Abstractions.External.Email;
using ClinicOS.Application.Common.Abstractions.External.Email.Constants;
using ClinicOS.Application.Common.Abstractions.External.Email.Models;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates;
using ClinicOS.Infrastructure.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ClinicOS.Infrastructure.External.Email;

/// <summary>
/// خدمة إرسال البريد الإلكتروني عبر SMTP
/// مخصصة لنظام ClinicOS مع دعم القوالب
/// </summary>
public class SmtpEmailSender : IEmailSender
{
    private readonly MailOptions _mailSettings;
    private readonly EmailTemplateEngine _templateEngine;

    public SmtpEmailSender(
        IOptions<MailOptions> mailSettings,
        EmailTemplateEngine templateEngine)
    {
        _mailSettings = mailSettings.Value;
        _templateEngine = templateEngine;
    }

    /// <summary>
    /// إرسال بريد إلكتروني مخصص
    /// </summary>
    public async Task SendEmailAsync(EmailRequest request)
    {
        var message = new MimeMessage();

        // اسم الراسل: اسم العيادة أو اسم مخصص
        var senderDisplayName = !string.IsNullOrWhiteSpace(request.SenderDisplayName)
            ? request.SenderDisplayName
            : _mailSettings.SenderName;

        message.From.Add(new MailboxAddress(senderDisplayName, _mailSettings.SenderEmail));

        if (request.To != null)
            message.To.AddRange(request.To.Select(MailboxAddress.Parse));

        if (request.Cc != null)
            message.Cc.AddRange(request.Cc.Select(MailboxAddress.Parse));

        if (request.Bcc != null)
            message.Bcc.AddRange(request.Bcc.Select(MailboxAddress.Parse));

        message.Subject = request.Subject;

        // بناء محتوى البريد مع دعم المرفقات
        if (request.IsHtml)
        {
            var htmlPart = new TextPart("html") { Text = request.Body };
            htmlPart.ContentType.Charset = "utf-8";
            htmlPart.ContentTransferEncoding = ContentEncoding.QuotedPrintable;

            if (request.Attachments != null && request.Attachments.Any())
            {
                var multipart = new Multipart("mixed");
                multipart.Add(htmlPart);

                foreach (var attachment in request.Attachments)
                {
                    if (attachment.Content != null && attachment.Content != Stream.Null)
                    {
                        if (attachment.Content.CanSeek)
                            attachment.Content.Position = 0;

                        var mimePart = new MimePart(attachment.ContentType ?? "application/octet-stream")
                        {
                            Content = new MimeContent(attachment.Content),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = attachment.FileName
                        };
                        multipart.Add(mimePart);
                    }
                }
                message.Body = multipart;
            }
            else
            {
                message.Body = htmlPart;
            }
        }
        else
        {
            var textPart = new TextPart("plain") { Text = request.Body };
            textPart.ContentType.Charset = "utf-8";
            textPart.ContentTransferEncoding = ContentEncoding.QuotedPrintable;
            message.Body = textPart;
        }

        using var client = new SmtpClient();
        try
        {
            var secureOption = _mailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
            if (_mailSettings.Port == 465) secureOption = SecureSocketOptions.SslOnConnect;

            await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, secureOption);

            if (!string.IsNullOrEmpty(_mailSettings.Username))
                await client.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password);

            await client.SendAsync(message);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }

    // ==========================================================
    // المحرك الأساسي لإرسال القوالب
    // ==========================================================

    private async Task SendTemplateEmailAsync<TModel>(
        string to,
        string subject,
        string templateName,
        TModel model) where TModel : BaseEmailTemplateModel
    {
        // 1. تحويل القالب إلى HTML
        var body = await _templateEngine.RenderTemplateAsync(templateName, model);

        // 2. تحضير طلب الإرسال مع اسم العيادة
        var request = new EmailRequest
        {
            To = new() { to },
            Subject = $"{subject} - {model.ClinicName}",
            Body = body,
            IsHtml = true,
            SenderDisplayName = model.ClinicName // يظهر للمستلم اسم العيادة
        };

        await SendEmailAsync(request);
    }

    // ==========================================================
    // دوال إرسال البريد المخصصة لنظام ClinicOS
    // ==========================================================

    public async Task SendEmailConfirmationAsync(string email, EmailConfirmationTemplateModel model)
        => await SendTemplateEmailAsync(email, "تأكيد بريدك الإلكتروني", EmailTemplateNames.EmailConfirmation, model);

    public async Task SendResetPasswordEmailAsync(string email, ResetPasswordTemplateModel model)
        => await SendTemplateEmailAsync(email, "إعادة تعيين كلمة المرور", EmailTemplateNames.ResetPassword, model);

    public async Task SendWelcomeEmailAsync(string email, WelcomeTemplateModel model)
        => await SendTemplateEmailAsync(email, "مرحباً بك في ClinicOS", EmailTemplateNames.Welcome, model);

    public async Task SendPasswordChangedEmailAsync(string email, PasswordChangedTemplateModel model)
        => await SendTemplateEmailAsync(email, "تم تغيير كلمة المرور", EmailTemplateNames.PasswordChanged, model);

    public async Task SendAccountLockedEmailAsync(string email, AccountLockedTemplateModel model)
        => await SendTemplateEmailAsync(email, "تم قفل حسابك", EmailTemplateNames.AccountLocked, model);

    public async Task SendAccountUnlockedEmailAsync(string email, AccountUnlockedTemplateModel model)
        => await SendTemplateEmailAsync(email, "تم فتح قفل حسابك", EmailTemplateNames.AccountUnlocked, model);

    public async Task SendRoleAssignedEmailAsync(string email, RoleAssignedTemplateModel model)
        => await SendTemplateEmailAsync(email, "تم تعيين دور جديد لك", EmailTemplateNames.RoleAssigned, model);

    public async Task SendRoleRemovedEmailAsync(string email, RoleRemovedTemplateModel model)
        => await SendTemplateEmailAsync(email, "تم إزالة دورك", EmailTemplateNames.RoleRemoved, model);

    public async Task SendAppointmentConfirmationAsync(string email, AppointmentConfirmationTemplateModel model)
        => await SendTemplateEmailAsync(email, "تأكيد حجز موعد", EmailTemplateNames.AppointmentConfirmation, model);

    public async Task SendAppointmentReminderAsync(string email, AppointmentReminderTemplateModel model)
        => await SendTemplateEmailAsync(email, "تذكير بموعدك", EmailTemplateNames.AppointmentReminder, model);
}