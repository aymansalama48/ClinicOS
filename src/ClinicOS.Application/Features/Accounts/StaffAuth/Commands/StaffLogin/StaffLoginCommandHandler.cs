using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.IdentityTemplats;
using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Application.Common.Abstractions.Web;
using ClinicOS.Application.Features.Accounts.StaffAuth.Shared;
using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Features.Accounts.StaffAuth.Commands.StaffLogin;

public sealed class StaffLoginCommandHandler(
    IStaffAuthService staffAuthService,
    IJobScheduler jobScheduler,
    IClientContext clientContext)
    : ICommandHandler<StaffLoginCommand, StaffAuthResponse>
{
    public async Task<Result<StaffAuthResponse>> Handle(
        StaffLoginCommand request,
        CancellationToken cancellationToken)
    {
        // 1. تنفيذ عملية تسجيل الدخول
        var result = await staffAuthService.LoginAsync(
            request.Email,
            request.Password,
            cancellationToken);

        // 2. إذا فشل تسجيل الدخول نرجع النتيجة فوراً
        if (result.IsFailure)
        {
            return result;
        }

        // 3. تجهيز بيانات إشعار تسجيل الدخول
        var templateModel = new LoginTemplateModel
        {
            UserName = result.Data!.FullName ?? string.Empty,
            LoginTime = result.Data.LoggedInAt,
            IpAddress = clientContext.IpAddress ?? string.Empty,
            UserAgent = clientContext.UserAgent ?? string.Empty
        };

        // 4. جدولة إرسال الإيميل كـ Background Job
        jobScheduler.Enqueue<IIdentityNotificationService>(
            sender => sender.SendLoginEmailAsync(
                request.Email,
                templateModel));

        // 5. إرجاع النتيجة فوراً دون انتظار إرسال الإيميل
        return result;
    }
}