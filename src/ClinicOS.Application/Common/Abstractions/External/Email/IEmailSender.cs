using ClinicOS.Application.Common.Abstractions.External.Email.Models;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates;

namespace ClinicOS.Application.Common.Abstractions.External.Email;

/// <summary>
/// واجهة خدمة إرسال البريد الإلكتروني
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// إرسال بريد إلكتروني مخصص
    /// </summary>
    Task SendEmailAsync(EmailRequest request);

    // ========== قوالب نظام ClinicOS ==========

    /// <summary>
    /// إرسال بريد تأكيد البريد الإلكتروني
    /// </summary>
    Task SendEmailConfirmationAsync(string email, EmailConfirmationTemplateModel model);

    /// <summary>
    /// إرسال بريد إعادة تعيين كلمة المرور
    /// </summary>
    Task SendResetPasswordEmailAsync(string email, ResetPasswordTemplateModel model);

    /// <summary>
    /// إرسال بريد الترحيب
    /// </summary>
    Task SendWelcomeEmailAsync(string email, WelcomeTemplateModel model);

    /// <summary>
    /// إرسال بريد إخطار تغيير كلمة المرور
    /// </summary>
    Task SendPasswordChangedEmailAsync(string email, PasswordChangedTemplateModel model);

    /// <summary>
    /// إرسال بريد إخطار قفل الحساب
    /// </summary>
    Task SendAccountLockedEmailAsync(string email, AccountLockedTemplateModel model);

    /// <summary>
    /// إرسال بريد إخطار فتح القفل
    /// </summary>
    Task SendAccountUnlockedEmailAsync(string email, AccountUnlockedTemplateModel model);

    /// <summary>
    /// إرسال بريد تعيين دور جديد
    /// </summary>
    Task SendRoleAssignedEmailAsync(string email, RoleAssignedTemplateModel model);

    /// <summary>
    /// إرسال بريد إزالة دور
    /// </summary>
    Task SendRoleRemovedEmailAsync(string email, RoleRemovedTemplateModel model);

    /// <summary>
    /// إرسال بريد تأكيد حجز موعد
    /// </summary>
    Task SendAppointmentConfirmationAsync(string email, AppointmentConfirmationTemplateModel model);

    /// <summary>
    /// إرسال بريد تذكير بالموعد
    /// </summary>
    Task SendAppointmentReminderAsync(string email, AppointmentReminderTemplateModel model);
}