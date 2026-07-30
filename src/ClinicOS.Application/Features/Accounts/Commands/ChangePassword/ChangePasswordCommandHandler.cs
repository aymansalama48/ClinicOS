using ClinicOS.Application.Common.Abstractions.External.Email;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates;
using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Identity.Security;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Features.Accounts.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    IPasswordService passwordService,
    ICurrentUser currentUser,
    IJobScheduler jobScheduler) : ICommandHandler<ChangePasswordCommand>
{
    public async Task<Result> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        // 1. جلب معرف البريد والمعرف الخاص بالمستخدم الحالي
        var userId = currentUser.UserId;
        var userEmail = currentUser.Email;

        if (userId is null || string.IsNullOrEmpty(userEmail))
        {
            return Result.Failure(UserErrors.NotFound);
        }

        // 2. تغيير كلمة المرور عبر الخدمة (إمرار Value للـ Guid)
        var result = await passwordService.ChangePasswordAsync(
            userId.Value,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        // 3. التحقق من نجاح عملية التغيير
        if (!result.IsSuccess)
        {
            return result;
        }

        // 4. جدولة إرسال بريد التأكيد في الخلفية
        jobScheduler.Enqueue<IEmailSender>(sender =>
            sender.SendPasswordChangedEmailAsync(
                userEmail,
                new PasswordChangedTemplateModel()));

        // 5. إرجاع استجابة النجاح فوراً
        return Result.Success();
    }
}