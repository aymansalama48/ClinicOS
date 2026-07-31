using ClinicOS.Application.Features.Accounts.PatientAuth.Shared;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Enums;

namespace ClinicOS.Application.Common.Abstractions.Identity.Authentication;

public interface IPatientAuthService
{
    /// <summary>
    /// التحقق من OTP وإنشاء/جلب المريض وإرجاع JWT قصير المدى
    /// يُستخدم لأي غرض (تأكيد حجز / مشاهدة حجوزات) حسب الـ purpose الممرر
    /// </summary>
    Task<Result<PatientAuthResponse>> LoginWithOtpAsync(
        string phoneNumber,
        string code,
        OtpPurpose purpose,   // 👈 مضافة
        CancellationToken cancellationToken = default);

    Task<Result<PatientAuthResponse>> RegisterPermanentAccountAsync(
        string email,
        string password,
        string phoneNumber,
        string otpCode,
        CancellationToken cancellationToken = default);

    Task<Result<PatientAuthResponse>> LoginWithEmailAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
    // IPatientAuthService.cs — إضافة Method جديدة
    Task<Result<PatientAuthResponse>> LoginWithGoogleAsync(
        string idToken,
        CancellationToken cancellationToken = default);
}

