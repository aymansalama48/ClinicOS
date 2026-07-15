using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Security;
using ClinicOS.Domain.Enums;
using ClinicOS.Domain.Security;
using ClinicOS.Infrastructure.Options;
using ClinicOS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace ClinicOS.Infrastructure.Identity.Security;

public class OtpService : IOtpService
{
    private readonly AppDbContext _context;
    private readonly OtpOptions _options;
    private readonly IDateTime _dateTime;

    public OtpService(AppDbContext context, IOptions<OtpOptions> options, IDateTime dateTime)
    {
        _context = context;
        _options = options.Value;
        _dateTime = dateTime;
    }

    public async Task<Result<OtpGenerationResult>> GenerateOtpAsync(
        string phoneNumber,
        OtpPurpose purpose,
        Guid? appointmentId,
        CancellationToken cancellationToken)
    {
        // 1. إلغاء أي كود سابق لسه شغال لنفس الرقم ونفس الغرض
        //    (يمنع وجود أكتر من كود صالح في نفس الوقت لنفس الرقم/الغرض)
        var oldActiveOtps = await _context.OtpVerifications
            .Where(o => o.Phone == phoneNumber
                     && o.Purpose == purpose
                     && !o.IsConsumed
                     && o.Expiry > _dateTime.Now)
            .ToListAsync(cancellationToken);

        foreach (var old in oldActiveOtps)
            old.IsConsumed = true;

        // 2. توليد كود عشوائي آمن كريبتوجرافيًا (مش System.Random العادي)
        var code = GenerateSecureNumericCode(_options.CodeLength);
        var now = _dateTime.Now;

        var otp = new OtpVerification
        {
            Id = Guid.CreateVersion7(),
            Phone = phoneNumber,
            Purpose = purpose,
            AppointmentId = appointmentId,
            CodeHash = HashCode(code, phoneNumber),
            Expiry = now.Add(_options.Expiry),
            NextResendAllowedAtUtc = now.Add(_options.ResendCooldown),
            AttemptsCount = 0,
            MaxAttempts = _options.MaxAttempts,
            IsConsumed = false
        };

        _context.OtpVerifications.Add(otp);
        await _context.SaveChangesAsync(cancellationToken);

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

    public async Task<Result> ValidateOtpAsync(
        string phoneNumber,
        string code,
        OtpPurpose purpose,
        CancellationToken cancellationToken)
    {
        var otp = await _context.OtpVerifications
            .Where(o => o.Phone == phoneNumber && o.Purpose == purpose && !o.IsConsumed)
            .OrderByDescending(o => o.Expiry)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp is null)
            return Result.Failure(OtpErrors.NotFound);

        if (otp.Expiry < _dateTime.Now)
            return Result.Failure(OtpErrors.Expired);

        if (otp.IsMaxAttemptsReached)
        {
            otp.IsConsumed = true; // اتقفل نهائي، لازم كود جديد
            await _context.SaveChangesAsync(cancellationToken);
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
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Failure(OtpErrors.InvalidCode);
        }

        otp.IsConsumed = true;
        otp.VerifiedAt = _dateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<OtpGenerationResult>> ResendOtpAsync(
        string phoneNumber,
        OtpPurpose purpose,
        CancellationToken cancellationToken)
    {
        var lastOtp = await _context.OtpVerifications
            .Where(o => o.Phone == phoneNumber && o.Purpose == purpose)
            .OrderByDescending(o => o.Expiry)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastOtp is not null && lastOtp.NextResendAllowedAtUtc > _dateTime.Now)
            return Result<OtpGenerationResult>.Failure(OtpErrors.ResendTooSoon);

        // إعادة الإرسال = توليد كود جديد بالكامل (أبسط وأأمن من محاولة إعادة استخدام القديم)
        return await GenerateOtpAsync(phoneNumber, purpose, lastOtp?.AppointmentId, cancellationToken);
    }

    private string HashCode(string code, string phoneNumber)
    {
        // بنضيف رقم الموبايل جوه الـ Hash عشان نفس الكود على رقمين مختلفين
        // يطلع Hash مختلف (يمنع مقارنة الكودات ببعض عبر الأرقام)
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.HashingSecret));
        var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{phoneNumber}:{code}"));
        return Convert.ToBase64String(bytes);
    }

    private static string GenerateSecureNumericCode(int length)
    {
        var min = (int)Math.Pow(10, length - 1);
        var max = (int)Math.Pow(10, length) - 1;
        return RandomNumberGenerator.GetInt32(min, max + 1).ToString();
    }
}