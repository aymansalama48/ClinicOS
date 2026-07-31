namespace ClinicOS.Application.Features.Accounts.StaffAuth.Commands.StaffGoogleLogin;

using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.IdentityTemplats;
using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Application.Common.Abstractions.Web;
using ClinicOS.Application.Features.Accounts.StaffAuth.Shared;
using ClinicOS.Domain.Common.Results;

public sealed class StaffGoogleLoginCommandHandler(
    IStaffAuthService staffAuthService,
    IJobScheduler jobScheduler,
    IClientContext clientContext) : ICommandHandler<StaffGoogleLoginCommand, StaffAuthResponse>
{
    public async Task<Result<StaffAuthResponse>> Handle(
        StaffGoogleLoginCommand request,
        CancellationToken cancellationToken)
    {
        // 1. التحقق من Google Token وتسجيل الدخول
        var result = await staffAuthService.LoginWithGoogleAsync(
            request.IdToken,
            cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        // 2. إرسال بريد إشعار بدخول الحساب عبر Google
        var templateModel = new LoginTemplateModel
        {
            UserName = result.Data!.FullName ?? string.Empty,
            IpAddress = clientContext.IpAddress ?? string.Empty,
            Device = clientContext.UserAgent ?? string.Empty,
            LoginTime = result.Data.LoggedInAt
        };

        jobScheduler.Enqueue<IIdentityNotificationService>(sender =>
            sender.SendLoginEmailAsync(result.Data.Email, templateModel));

        return result;
    }
}