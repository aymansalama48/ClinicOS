using ClinicOS.Application.Common.Abstractions.External.Email;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates;
using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.Identity.Security;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Features.Accounts.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(
    IPasswordService passwordService,
    IJobScheduler jobScheduler) : ICommandHandler<ResetPasswordCommand>
{
    public async Task<Result> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // 1. إعادة تعيين كلمة المرور عبر خدمة الهوية
        var result = await passwordService.ResetPasswordAsync(
            request.Email,
            request.Token,
            request.NewPassword,
            cancellationToken);

        // 2. التحقق من نجاح العملية قبل متابعة الإرسال
        if (!result.IsSuccess)
        {
            return result;
        }

        // 3. جدولة إرسال بريد التأكيد في الخلفية (تاريخ التغيير سيتحدد تلقائياً داخل الخدمة)
        jobScheduler.Enqueue<IEmailSender>(sender =>
            sender.SendPasswordChangedEmailAsync(
                request.Email,
                new PasswordChangedTemplateModel()));

        // 4. إرجاع استجابة النجاح فوراً
        return Result.Success();
    }
}