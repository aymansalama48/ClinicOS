using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Errors.Identity;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.OtpVerification;
using ClinicOS.Domain.Enums;
using ClinicOS.Infrastructure.Options;
using ClinicOS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace ClinicOS.Infrastructure.Identity.Security;

/// <summary>
/// تنفيذ خدمة OTP — توليد وتحقق وإعادة إرسال أكواد التحقق برقم الموبايل
/// </summary>
public class OtpService(
    AppDbContext context,
    IOptions<OtpOptions> options,
    IDateTime dateTime) : IOtpService
{
    /// <summary>
    /// توليد OTP جديد — بيلغي أي كود سابق شغال لنفس الرقم/الغرض ويحفظ الكود بشكل مشفر
    /// </summary>
    public async Task<Result<OtpGenerationResult>> GenerateOtpAsync(
        string phoneNumber,
        OtpPurpose purpose,
        Guid? appointmentId,
        CancellationToken cancellationToken)
    {
        // 1. إلغاء أي كود سابق لسه شغال لنفس الرقم ونفس الغرض
        //    (يمنع وجود أكتر من كود صالح في نفس الوقت لنفس الرقم/الغرض)
        var oldActiveOtps = await context.OtpVerifications
            .Where(o => o.Phone == phoneNumber
                     && o.Purpose == purpose
                     && !o.IsConsumed
                     && o.Expiry > dateTime.Now)
            .ToListAsync(cancellationToken);

        foreach (var old in oldActiveOtps)
            old.IsConsumed = true;

        // 2. توليد كود عشوائي آمن كريبتوجرافيًا (مش System.Random العادي)
        var code = GenerateSecureNumericCode(options.Value.CodeLength);
        var now = dateTime.Now;

        var otp = new OtpVerification
        {
            Id = Guid.CreateVersion7(),
            Phone = phoneNumber,
            Purpose = purpose,
            AppointmentId = appointmentId,
            CodeHash = HashCode(code, phoneNumber),
            Expiry = now.Add(options.Value.Expiry),
            NextResendAllowedAtUtc = now.Add(options.Value.ResendCooldown),
            AttemptsCount = 0,
            MaxAttempts = options.Value.MaxAttempts,
            IsConsumed = false
        };

        context.OtpVerifications.Add(otp);
        await context.SaveChangesAsync(cancellationToken);

        // الكود الحقيقي بيترجع هنا بس — اللي بينادي الميثود دي هو المسؤول عن إرسال SMS
        // (الـ OtpService نفسها ملهاش أي علاقة بطريقة الإرسال)
        return Result<OtpGenerationResult>.Success(new OtpGenerationResult
        {
            Code = code,
            OtpId = otp.Id,
            ExpiresAtUtc = otp.Expiry,
            NextResendAllowedAtUtc = otp.NextResendAllowedAtUtc
        });
    }

    /// <summary>
    /// التحقق من الـ OTP — مع عدد محاولات محدود ومقارنة آمنة ضد الـ Timing Attacks
    /// </summary>
    public async Task<Result> ValidateOtpAsync(
        string phoneNumber,
        string code,
        OtpPurpose purpose,
        CancellationToken cancellationToken)
    {
        var otp = await context.OtpVerifications
            .Where(o => o.Phone == phoneNumber && o.Purpose == purpose && !o.IsConsumed)
            .OrderByDescending(o => o.Expiry)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp is null)
            return Result.Failure(OtpErrors.NotFound);

        if (otp.Expiry < dateTime.Now)
            return Result.Failure(OtpErrors.Expired);

        if (otp.IsMaxAttemptsReached)
        {
            otp.IsConsumed = true; // اتقفل نهائي، لازم كود جديد
            await context.SaveChangesAsync(cancellationToken);
            return Result.Failure(OtpErrors.MaxAttemptsExceeded);
        }

        var expectedHash = HashCode(code, phoneNumber);

        // مقارنة بوقت ثابت (Constant-Time) عشان تمنع Timing Attacks
        var isMatch = CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(expectedHash),
            Convert.FromBase64String(otp.CodeHash));

        if (!isMatch)
        {
            otp.AttemptsCount++;
            await context.SaveChangesAsync(cancellationToken);
            return Result.Failure(OtpErrors.InvalidCode);
        }

        otp.IsConsumed = true;
        otp.VerifiedAt = dateTime.Now;
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// إعادة إرسال OTP — مع احترام فترة الـ Cooldown بين كل إرسال والتاني
    /// </summary>
    public async Task<Result<OtpGenerationResult>> ResendOtpAsync(
        string phoneNumber,
        OtpPurpose purpose,
        CancellationToken cancellationToken)
    {
        var lastOtp = await context.OtpVerifications
            .Where(o => o.Phone == phoneNumber && o.Purpose == purpose)
            .OrderByDescending(o => o.Expiry)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastOtp is not null && lastOtp.NextResendAllowedAtUtc > dateTime.Now)
            return Result<OtpGenerationResult>.Failure(OtpErrors.ResendTooSoon);

        // إعادة الإرسال = توليد كود جديد بالكامل (أبسط وأأمن من محاولة إعادة استخدام القديم)
        return await GenerateOtpAsync(phoneNumber, purpose, lastOtp?.AppointmentId, cancellationToken);
    }

    // تشفير الكود مع رقم الموبايل — نفس الكود على رقمين مختلفين يطلع Hash مختلف
    private string HashCode(string code, string phoneNumber)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(options.Value.HashingSecret));
        var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{phoneNumber}:{code}"));
        return Convert.ToBase64String(bytes);
    }

    // توليد كود رقمي آمن كريبتوجرافيًا
    private static string GenerateSecureNumericCode(int length)
    {
        var min = (int)Math.Pow(10, length - 1);
        var max = (int)Math.Pow(10, length) - 1;
        return RandomNumberGenerator.GetInt32(min, max + 1).ToString();
    }
}