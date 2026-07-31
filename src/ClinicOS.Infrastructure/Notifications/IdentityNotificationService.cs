namespace ClinicOS.Infrastructure.Notifications;

using System.Reflection;
using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.External.Client;
using ClinicOS.Application.Common.Abstractions.External.Client.Models;
using ClinicOS.Application.Common.Abstractions.External.Email;
using ClinicOS.Application.Common.Abstractions.External.Email.Constants;
using ClinicOS.Application.Common.Abstractions.External.Email.Models;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.IdentityTemplats;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Infrastructure.External.Email;
using ClinicOS.Infrastructure.Options;
using Microsoft.Extensions.Options;

public sealed class IdentityNotificationService : IIdentityNotificationService
{
    private readonly IEmailSender _emailSender;
    private readonly EmailTemplateEngine _templateEngine;
    private readonly BaseUrlOptions _baseUrlOptions;
    private readonly IDateTime _dateTimeProvider;
    private readonly IUserAgentParser _userAgentParser;
    private readonly IGeoLocationService _geoLocationService;

    public IdentityNotificationService(
        IEmailSender emailSender,
        EmailTemplateEngine templateEngine,
        IOptions<BaseUrlOptions> baseUrlOptions,
        IDateTime dateTimeProvider,
        IUserAgentParser userAgentParser,
        IGeoLocationService geoLocationService)
    {
        _emailSender = emailSender;
        _templateEngine = templateEngine;
        _baseUrlOptions = baseUrlOptions.Value;
        _dateTimeProvider = dateTimeProvider;
        _userAgentParser = userAgentParser;
        _geoLocationService = geoLocationService;
    }

    public async Task SendLoginEmailAsync(string email, LoginTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";
        if (model.LoginTime == default) model.LoginTime = _dateTimeProvider.Now;

        // تعبئة بيانات العميل (الموجودة في هذا الموديل)
        await PopulateClientInfoAsync(model, model.UserAgent, model.IpAddress);

        await SendTemplateEmailAsync(email, "إشعار تسجيل دخول جديد", EmailTemplateNames.Login, model);
    }

    public async Task SendEmailConfirmationAsync(string email, EmailConfirmationTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";
        if (string.IsNullOrWhiteSpace(model.Email)) model.Email = email;
        if (string.IsNullOrWhiteSpace(model.ConfirmationLink)) model.ConfirmationLink = $"{_baseUrlOptions.Frontend}/confirm-email";

        // تعبئة بيانات العميل من خصائص الموديل نفسه (لأنك وضعتها فيه)
        await PopulateClientInfoAsync(model, model.UserAgent, model.IpAddress);

        await SendTemplateEmailAsync(email, "تأكيد بريدك الإلكتروني", EmailTemplateNames.EmailConfirmation, model);
    }

    public async Task SendResetPasswordEmailAsync(string email, ResetPasswordTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";
        if (string.IsNullOrWhiteSpace(model.Email)) model.Email = email;
        if (string.IsNullOrWhiteSpace(model.ResetLink)) model.ResetLink = $"{_baseUrlOptions.Frontend}/reset-password";

        // تعبئة بيانات العميل من خصائص الموديل نفسه
        await PopulateClientInfoAsync(model, model.UserAgent, model.IpAddress);

        await SendTemplateEmailAsync(email, "إعادة تعيين كلمة المرور", EmailTemplateNames.ResetPassword, model);
    }

    public async Task SendWelcomeEmailAsync(string email, WelcomeTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";
        if (string.IsNullOrWhiteSpace(model.LoginUrl)) model.LoginUrl = $"{_baseUrlOptions.Frontend}/login";

        await SendTemplateEmailAsync(email, "مرحباً بك في ClinicOS", EmailTemplateNames.Welcome, model);
    }

    public async Task SendPasswordChangedEmailAsync(string email, PasswordChangedTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";
        if (model.ChangedAt == default) model.ChangedAt = _dateTimeProvider.Now;

        // تعبئة بيانات العميل من خصائص الموديل نفسه
        await PopulateClientInfoAsync(model, model.UserAgent, model.IpAddress);

        await SendTemplateEmailAsync(email, "تم تغيير كلمة المرور", EmailTemplateNames.PasswordChanged, model);
    }

    public async Task SendAccountLockedEmailAsync(string email, AccountLockedTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";
        if (string.IsNullOrWhiteSpace(model.UnlockUrl)) model.UnlockUrl = $"{_baseUrlOptions.Frontend}/support";

        await SendTemplateEmailAsync(email, "تم قفل حسابك", EmailTemplateNames.AccountLocked, model);
    }

    public async Task SendAccountUnlockedEmailAsync(string email, AccountUnlockedTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";
        if (string.IsNullOrWhiteSpace(model.LoginUrl)) model.LoginUrl = $"{_baseUrlOptions.Frontend}/login";

        await SendTemplateEmailAsync(email, "تم فتح قفل حسابك", EmailTemplateNames.AccountUnlocked, model);
    }

    public async Task SendRoleAssignedEmailAsync(string email, RoleAssignedTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";
        if (string.IsNullOrWhiteSpace(model.RoleName)) model.RoleName = "مستخدم";
        if (string.IsNullOrWhiteSpace(model.DashboardUrl)) model.DashboardUrl = $"{_baseUrlOptions.Frontend}/dashboard";

        await SendTemplateEmailAsync(email, "تم تعيين دور جديد لك", EmailTemplateNames.RoleAssigned, model);
    }

    public async Task SendRoleRemovedEmailAsync(string email, RoleRemovedTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName)) model.UserName = "المستخدم العزيز";
        if (string.IsNullOrWhiteSpace(model.RoleName)) model.RoleName = "مستخدم";

        await SendTemplateEmailAsync(email, "تم إزالة دورك", EmailTemplateNames.RoleRemoved, model);
    }

    // ============================================================
    // الدوال المساعدة الموحدة (Reflection لتعمل مع النماذج التي تحتوي على الخصائص فقط)
    // ============================================================

    private async Task PopulateClientInfoAsync<TModel>(TModel model, string? userAgent, string? ipAddress)
        where TModel : class
    {
        // تعيين القيم الافتراضية
        SetPropertyValue(model, "IpAddress", string.IsNullOrWhiteSpace(ipAddress) ? "غير معروف" : ipAddress);
        SetPropertyValue(model, "Device", "جهاز غير معروف");
        SetPropertyValue(model, "Location", "موقع غير معروف");

        // تحليل الجهاز
        try
        {
            if (!string.IsNullOrWhiteSpace(userAgent))
            {
                var deviceInfo = _userAgentParser.Parse(userAgent);
                SetPropertyValue(model, "Device", BuildDeviceName(deviceInfo));
            }
        }
        catch { /* تجاهل الأخطاء */ }

        // تحديد الموقع
        try
        {
            var currentIp = string.IsNullOrWhiteSpace(ipAddress) ? "غير معروف" : ipAddress;
            if (currentIp != "غير معروف")
            {
                var location = await _geoLocationService.GetLocationAsync(currentIp, CancellationToken.None);
                if (!string.IsNullOrWhiteSpace(location))
                {
                    SetPropertyValue(model, "Location", location);
                }
            }
        }
        catch { /* تجاهل الأخطاء */ }
    }

    // دالة آمنة: لو الخاصية مش موجودة في الموديل، مش هيعمل حاجة و مش هيطلع خطأ
    private static void SetPropertyValue<TModel>(TModel model, string propertyName, string value)
        where TModel : class
    {
        if (model == null) return;

        var prop = typeof(TModel).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(model, value);
        }
    }

    private static string BuildDeviceName(ClientDeviceInfo device)
    {
        var browser = device.Browser?.Trim();
        var operatingSystem = device.OperatingSystem?.Trim();

        if (string.IsNullOrWhiteSpace(browser) && string.IsNullOrWhiteSpace(operatingSystem))
            return "جهاز غير معروف";

        if (string.IsNullOrWhiteSpace(browser)) return operatingSystem!;
        if (string.IsNullOrWhiteSpace(operatingSystem)) return browser!;

        return $"{browser} on {operatingSystem}";
    }

    private async Task SendTemplateEmailAsync<TModel>(string to, string subject, string templateName, TModel model)
        where TModel : BaseEmailTemplateModel
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
    public async Task SendStaffInvitationEmailAsync(string email, StaffInvitationTemplateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.RoleName)) model.RoleName = "موظف";
        if (string.IsNullOrWhiteSpace(model.AdminName)) model.AdminName = "إدارة النظام";
        if (string.IsNullOrWhiteSpace(model.InvitedEmail)) model.InvitedEmail = email;

        // 💡 بناء الرابط ديناميكياً بناءً على إعدادات الـ Frontend والـ Token الممرر
        if (string.IsNullOrWhiteSpace(model.InvitationLink) && !string.IsNullOrWhiteSpace(model.Token))
        {
            model.InvitationLink = $"{_baseUrlOptions.Frontend}/accept-invitation?token={model.Token}";
        }

        await SendTemplateEmailAsync(
            email,
            "دعوة للانضمام إلى فريق العمل",
            "StaffInvitation", // اسم ملف الـ HTML
            model);
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