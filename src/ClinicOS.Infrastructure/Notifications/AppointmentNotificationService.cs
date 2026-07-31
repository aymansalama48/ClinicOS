namespace ClinicOS.Infrastructure.Notifications;

using ClinicOS.Application.Common.Abstractions.External.Email;
using ClinicOS.Application.Common.Abstractions.External.Email.Constants;
using ClinicOS.Application.Common.Abstractions.External.Email.Models;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.AppointmentTemplats;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Infrastructure.External.Email;
using ClinicOS.Infrastructure.Options;
using Microsoft.Extensions.Options;

public class AppointmentNotificationService : IAppointmentNotificationService
{
    private readonly IEmailSender _emailSender;
    private readonly EmailTemplateEngine _templateEngine;
    private readonly BaseUrlOptions _baseUrlOptions;

    public AppointmentNotificationService(
        IEmailSender emailSender,
        EmailTemplateEngine templateEngine,
        IOptions<BaseUrlOptions> baseUrlOptions)
    {
        _emailSender = emailSender;
        _templateEngine = templateEngine;
        _baseUrlOptions = baseUrlOptions.Value;
    }

    public async Task SendAppointmentConfirmationAsync(string email, AppointmentConfirmationTemplateModel model)
    {
        await SendTemplateEmailAsync(email, "تأكيد حجز موعد", EmailTemplateNames.AppointmentConfirmation, model);
    }

    public async Task SendAppointmentReminderAsync(string email, AppointmentReminderTemplateModel model)
    {
        await SendTemplateEmailAsync(email, "تذكير بموعدك", EmailTemplateNames.AppointmentReminder, model);
    }

    private async Task SendTemplateEmailAsync<TModel>(
        string to,
        string subject,
        string templateName,
        TModel model) where TModel : BaseEmailTemplateModel
    {
        PopulateDefaultBaseData(model);

        var body = await _templateEngine.RenderTemplateAsync(templateName, model);

        var request = new EmailRequest
        {
            To = new() { to },
            Subject = $"{subject} - {model.ClinicName}",
            Body = body,
            IsHtml = true,
            SenderDisplayName = model.ClinicName
        };

        await _emailSender.SendEmailAsync(request);
    }

    private void PopulateDefaultBaseData(BaseEmailTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.ClinicName)) model.ClinicName = "ClinicOS";
        if (string.IsNullOrWhiteSpace(model.SupportEmail)) model.SupportEmail = "support@clinicos.com";
        if (string.IsNullOrWhiteSpace(model.ClinicPhoneNumber)) model.ClinicPhoneNumber = "+201000000000";
        if (string.IsNullOrWhiteSpace(model.Address)) model.Address = "القاهرة، مصر";
        if (string.IsNullOrWhiteSpace(model.WebsiteUrl)) model.WebsiteUrl = _baseUrlOptions.Frontend;
        if (string.IsNullOrWhiteSpace(model.FacebookUrl)) model.FacebookUrl = "https://facebook.com/clinicos";
        if (string.IsNullOrWhiteSpace(model.InstagramUrl)) model.InstagramUrl = "https://instagram.com/clinicos";
        if (string.IsNullOrWhiteSpace(model.WhatsAppNumber)) model.WhatsAppNumber = "+201000000000";
    }
}