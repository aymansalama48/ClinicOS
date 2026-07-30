using ClinicOS.Application.Common.Abstractions.External.Email;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates;
using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.External.Routing;
using ClinicOS.Application.Common.Abstractions.Identity.Security;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Features.Accounts.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IPasswordService passwordService,
    IApplicationUrlService urlService,
    IJobScheduler jobScheduler) : ICommandHandler<ForgotPasswordCommand>
{
    public async Task<Result> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // 1. إنشاء رمز إعادة تعيين كلمة المرور (Reset Token)
        var result = await passwordService.ForgotPasswordAsync(
            request.Email,
            cancellationToken);

        // 2. حماية أمنية: عدم كشف وجود البريد الإلكتروني من عدمه للمستخدم
        if (!result.IsSuccess)
        {
            return result;
        }

        if (string.IsNullOrEmpty(result.Data))
        {
            return Result.Success();
        }

        // 3. إنشاء رابط إعادة التعيين الخاص بالواجهة الأمامية
        var resetLink = urlService.GeneratePasswordResetUrl(
            request.Email,
            result.Data);

        // 4. تجهيز نموذج قالب البريد الإلكتروني
        var templateModel = new ResetPasswordTemplateModel
        {
            ResetLink = resetLink
        };

        // 5. جدولة إرسال البريد كـ Background Job لضمان السرعة وعدم التعطيل
        jobScheduler.Enqueue<IEmailSender>(
            sender => sender.SendResetPasswordEmailAsync(
                request.Email,
                templateModel));

        // 6. إرجاع استجابة النجاح فوراً
        return Result.Success();
    }
}