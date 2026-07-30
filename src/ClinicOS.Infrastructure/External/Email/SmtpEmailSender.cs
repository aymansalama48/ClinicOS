using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.External.Email;
using ClinicOS.Application.Common.Abstractions.External.Email.Constants;
using ClinicOS.Application.Common.Abstractions.External.Email.Models;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates;
using ClinicOS.Infrastructure.Core;
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
    private readonly BaseUrlOptions _baseUrlOptions;
    private readonly EmailTemplateEngine _templateEngine;
    private readonly IDateTime _dateTimeProvider; // <--- حقن خدمة التاريخ والوقت

    public SmtpEmailSender(
        IOptions<MailOptions> mailSettings,
        IOptions<BaseUrlOptions> baseUrlOptions,
        EmailTemplateEngine templateEngine,
        IDateTime dateTimeProvider) // <--- حقن في الـ Constructor
    {
        _mailSettings = mailSettings.Value;
        _baseUrlOptions = baseUrlOptions.Value;
        _templateEngine = templateEngine;
        _dateTimeProvider = dateTimeProvider;
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
        PopulateDefaultBaseData(model);

        // 1. تحويل القالب إلى HTML
        var body = await _templateEngine.RenderTemplateAsync(templateName, model);

        // 2. تحضير طلب الإرسال مع اسم العيادة
        var request = new EmailRequest
        {
            To = new() { to },
            Subject = $"{subject} - {model.ClinicName}",
            Body = body,
            IsHtml = true,
            SenderDisplayName = model.ClinicName
        };

        await SendEmailAsync(request);
    }

    /// <summary>
    /// تعبئة بيانات افتراضية/وهمية للنموذج الأب BaseEmailTemplateModel
    /// </summary>
    private void PopulateDefaultBaseData(BaseEmailTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.ClinicName))
            model.ClinicName = "ClinicOS";

        if (string.IsNullOrWhiteSpace(model.ClinicLogoUrl))
            model.ClinicLogoUrl = $"{_baseUrlOptions.Frontend}/assets/images/logo.png";

        if (string.IsNullOrWhiteSpace(model.SupportEmail))
            model.SupportEmail = "support@clinicos.com";

        if (string.IsNullOrWhiteSpace(model.PhoneNumber))
            model.PhoneNumber = "+201000000000";

        if (string.IsNullOrWhiteSpace(model.Address))
            model.Address = "القاهرة، مصر";

        if (string.IsNullOrWhiteSpace(model.WebsiteUrl))
            model.WebsiteUrl = _baseUrlOptions.Frontend;

        if (string.IsNullOrWhiteSpace(model.FacebookUrl))
            model.FacebookUrl = "https://facebook.com/clinicos";

        if (string.IsNullOrWhiteSpace(model.InstagramUrl))
            model.InstagramUrl = "https://instagram.com/clinicos";

        if (string.IsNullOrWhiteSpace(model.WhatsAppNumber))
            model.WhatsAppNumber = "+201000000000";
    }

    // ==========================================================
    // دوال إرسال البريد المخصصة لنظام ClinicOS
    // ==========================================================

    public async Task SendEmailConfirmationAsync(string email, EmailConfirmationTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";
        if (string.IsNullOrWhiteSpace(model.Email)) model.Email = email;
        if (string.IsNullOrWhiteSpace(model.ConfirmationLink))
            model.ConfirmationLink = $"{_baseUrlOptions.Frontend}/confirm-email";

        await SendTemplateEmailAsync(email, "تأكيد بريدك الإلكتروني", EmailTemplateNames.EmailConfirmation, model);
    }

    public async Task SendResetPasswordEmailAsync(string email, ResetPasswordTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";
        if (string.IsNullOrWhiteSpace(model.Email)) model.Email = email;
        if (string.IsNullOrWhiteSpace(model.ResetLink))
            model.ResetLink = $"{_baseUrlOptions.Frontend}/reset-password";

        await SendTemplateEmailAsync(email, "إعادة تعيين كلمة المرور", EmailTemplateNames.ResetPassword, model);
    }

    public async Task SendWelcomeEmailAsync(string email, WelcomeTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";
        if (string.IsNullOrWhiteSpace(model.LoginUrl))
            model.LoginUrl = $"{_baseUrlOptions.Frontend}/login";

        await SendTemplateEmailAsync(email, "مرحباً بك في ClinicOS", EmailTemplateNames.Welcome, model);
    }

    public async Task SendPasswordChangedEmailAsync(string email, PasswordChangedTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";

        // استخدام خدمة التاريخ والتوقيت المعتمدة
        if (model.ChangedAt == default) model.ChangedAt = _dateTimeProvider.Now;

        await SendTemplateEmailAsync(email, "تم تغيير كلمة المرور", EmailTemplateNames.PasswordChanged, model);
    }

    public async Task SendAccountLockedEmailAsync(string email, AccountLockedTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";
        if (string.IsNullOrWhiteSpace(model.UnlockUrl))
            model.UnlockUrl = $"{_baseUrlOptions.Frontend}/support";

        await SendTemplateEmailAsync(email, "تم قفل حسابك", EmailTemplateNames.AccountLocked, model);
    }

    public async Task SendAccountUnlockedEmailAsync(string email, AccountUnlockedTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";
        if (string.IsNullOrWhiteSpace(model.LoginUrl))
            model.LoginUrl = $"{_baseUrlOptions.Frontend}/login";

        await SendTemplateEmailAsync(email, "تم فتح قفل حسابك", EmailTemplateNames.AccountUnlocked, model);
    }

    public async Task SendRoleAssignedEmailAsync(string email, RoleAssignedTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";
        if (string.IsNullOrWhiteSpace(model.RoleName)) model.RoleName = "مستخدم";
        if (string.IsNullOrWhiteSpace(model.DashboardUrl))
            model.DashboardUrl = $"{_baseUrlOptions.Frontend}/dashboard";

        await SendTemplateEmailAsync(email, "تم تعيين دور جديد لك", EmailTemplateNames.RoleAssigned, model);
    }

    public async Task SendRoleRemovedEmailAsync(string email, RoleRemovedTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";
        if (string.IsNullOrWhiteSpace(model.RoleName)) model.RoleName = "مستخدم";

        await SendTemplateEmailAsync(email, "تم إزالة دورك", EmailTemplateNames.RoleRemoved, model);
    }

    public async Task SendAppointmentConfirmationAsync(string email, AppointmentConfirmationTemplateModel model)
    {
        await SendTemplateEmailAsync(email, "تأكيد حجز موعد", EmailTemplateNames.AppointmentConfirmation, model);
    }

    public async Task SendAppointmentReminderAsync(string email, AppointmentReminderTemplateModel model)
    {
        await SendTemplateEmailAsync(email, "تذكير بموعدك", EmailTemplateNames.AppointmentReminder, model);
    }
}