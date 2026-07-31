namespace ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ChangePassword;

using ClinicOS.Application.Common.Abstractions.External.Email;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.IdentityTemplats;
using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Identity.Security;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Application.Common.Abstractions.Web;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Domain.Common.Results;

public sealed class ChangePasswordCommandHandler(
    IPasswordService passwordService,
    ICurrentUser currentUser,
    IJobScheduler jobScheduler,
    IClientContext clientContext) : ICommandHandler<ChangePasswordCommand>
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

        // 2. تغيير كلمة المرور عبر الخدمة
        var result = await passwordService.ChangePasswordAsync(
            userId.Value,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        // 3. التحقق من نجاح عملية التغيير
        if (result.IsFailure)
        {
            return result;
        }

        // 4. تجهيز نموذج البريد الإلكتروني مع بيانات المستخدم والاتصال
        var templateModel = new PasswordChangedTemplateModel
        {
            UserName = currentUser.FullName ?? string.Empty,
            IpAddress = clientContext.IpAddress ?? string.Empty,
            UserAgent = clientContext.UserAgent ?? string.Empty
        };

        // 5. جدولة إرسال بريد التأكيد في الخلفية
        jobScheduler.Enqueue<IIdentityNotificationService>(sender =>
            sender.SendPasswordChangedEmailAsync(
                userEmail,
                templateModel));

        // 6. إرجاع استجابة النجاح فوراً
        return Result.Success("تم تغيير كلمة المرور بنجاح");
    }
}