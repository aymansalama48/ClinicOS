namespace ClinicOS.Application.Features.Otps.Commands.RequestOtp;

using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Domain.Common.Results;

public sealed class RequestOtpCommandHandler(
    IOtpService otpService,
    IJobScheduler jobScheduler) : ICommandHandler<RequestOtpCommand, RequestOtpResponse>
{
    public async Task<Result<RequestOtpResponse>> Handle(
        RequestOtpCommand request,
        CancellationToken cancellationToken)
    {
        // 1. توليد الـ OTP وتخزينه
        var otpResult = await otpService.GenerateOtpAsync(
            request.PhoneNumber,
            request.Purpose,
            request.AppointmentId,
            cancellationToken);

        // 💡 تصحيح 1 و 2: إمرار قائمة الأخطاء Errors واستخدام Result<T>.Failure
        if (otpResult.IsFailure)
        {
            return Result<RequestOtpResponse>.Failure(otpResult.Errors);
        }

        var otpData = otpResult.Data!;

        // 2. إرسال الـ SMS في الخلفية
        //jobScheduler.Enqueue<ISmsNotificationService>(sender =>
        //    sender.SendOtpSmsAsync(request.PhoneNumber, otpData.Code));


        Console.WriteLine(@$"
----------------------------------
otpData.Code: {otpData.Code}
phoneNumber: {request.PhoneNumber}
----------------------------------
");

        // 💡 تصحيح 3: تغليف الاستجابة بـ Result<T>.Success
        var response = new RequestOtpResponse(
            otpData.ExpiresAtUtc,
            otpData.NextResendAllowedAtUtc);

        return Result<RequestOtpResponse>.Success(response);
    }
}