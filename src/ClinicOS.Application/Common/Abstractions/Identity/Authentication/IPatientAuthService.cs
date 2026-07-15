using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Common.Abstractions.Identity.Authentication;

public interface IPatientAuthService
{
    /// <summary>
    /// التحقق من OTP وإنشاء/جلب المريض وإرجاع JWT قصير المدى (للحجز أو عرض الحجوزات)
    /// </summary>
    Task<Result<PatientAuthResponse>> VerifyOtpAndLoginAsync(
        string phoneNumber,
        string code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// إنشاء حساب دائم للمريض (Email + Password) وربطه ببيانات Patient القديمة بعد تأكيد الـ OTP
    /// </summary>
    Task<Result<PatientAuthResponse>> RegisterPermanentAccountAsync(
        string email,
        string password,
        string phoneNumber,
        string otpCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// تسجيل دخول المريض الذي يمتلك حساباً دائماً باستخدام البريد وكلمة السر
    /// </summary>
    Task<Result<PatientAuthResponse>> LoginWithEmailAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}

public record PatientAuthResponse(
    Guid PatientId,
    string AccessToken,
    int ExpiresInSeconds,
    bool IsPermanentAccount
);