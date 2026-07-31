namespace ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ForgotPassword;

using ClinicOS.Application.Common.Abstractions.External.Email;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.IdentityTemplats;
using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.External.Routing;
using ClinicOS.Application.Common.Abstractions.Identity.Security;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Application.Common.Abstractions.Web;
using ClinicOS.Domain.Common.Results;

public sealed class ForgotPasswordCommandHandler(
    IPasswordService passwordService,
    IApplicationUrlService urlService,
    IJobScheduler jobScheduler,
    IClientContext clientContext) : ICommandHandler<ForgotPasswordCommand>
{
    public async Task<Result> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // 1. إنشاء رمز إعادة تعيين كلمة المرور واسترجاع بيانات الحساب
        var result = await passwordService.ForgotPasswordAsync(
            request.Email,
            cancellationToken);

        // 2. حماية أمنية: عدم كشف وجود البريد الإلكتروني من عدمه للمستخدم
        if (result.IsFailure)
        {
            return result;
        }

        if (result.Data is null || string.IsNullOrEmpty(result.Data))
        {
            return Result.Success();
        }

        // 3. إنشاء رابط إعادة التعيين الخاص بالواجهة الأمامية
        var resetLink = urlService.GeneratePasswordResetUrl(
            request.Email,
            result.Data);

        // 4. تجهيز نموذج قالب البريد الإلكتروني وسحب IP و UserAgent من الـ ClientContext
        var templateModel = new ResetPasswordTemplateModel
        {
            ResetLink = resetLink,
            IpAddress = clientContext.IpAddress ?? string.Empty,
            UserAgent = clientContext.UserAgent ?? string.Empty
        };

        // 5. جدولة إرسال البريد كـ Background Job لضمان السرعة وعدم التعطيل
        jobScheduler.Enqueue<IIdentityNotificationService>(
            sender => sender.SendResetPasswordEmailAsync(
                request.Email,
                templateModel));

        // 6. إرجاع استجابة النجاح فوراً
        return Result.Success();
    }
}