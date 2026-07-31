namespace ClinicOS.Application.Features.Otps.Commands.RequestOtp;

public sealed record RequestOtpResponse(
    DateTime ExpiresAtUtc,
    DateTime NextResendAllowedAtUtc);