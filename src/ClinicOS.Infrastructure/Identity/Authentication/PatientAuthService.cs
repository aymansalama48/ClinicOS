using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Identity.Tokens;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Patients;
using ClinicOS.Domain.Enums;
using ClinicOS.Domain.Security;
using ClinicOS.Infrastructure.Persistence.Data;
using ClinicOS.Infrastructure.Persistence.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicOS.Infrastructure.Identity.Authentication;

/// <summary>
/// تنفيذ خدمة مصادقة المريض (Patient)
/// يدعم: تسجيل الدخول بـ OTP (بدون حساب)، إنشاء حساب دائم، وتسجيل الدخول بالبريد
/// </summary>
public class PatientAuthService : IPatientAuthService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IOtpService _otpService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDateTime _dateTime;
    private readonly ILogger<PatientAuthService> _logger;

    // مدة صلاحية توكن المريض (قصيرة المدى - مثلاً ساعة واحدة)
    private const int PatientTokenExpirySeconds = 3600;

    public PatientAuthService(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOtpService otpService,
        IJwtTokenGenerator jwtTokenGenerator,
        IDateTime dateTime,
        ILogger<PatientAuthService> logger)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _otpService = otpService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dateTime = dateTime;
        _logger = logger;
    }

    /// <summary>
    /// الخطوة الأولى والأساسية: التحقق من OTP وإنشاء/جلب المريض وإرجاع JWT قصير المدى
    /// تُستخدم في: (1) تأكيد الحجز، (2) مشاهدة الحجوزات، (3) أي وصول مؤقت للمريض
    /// </summary>
    public async Task<Result<PatientAuthResponse>> VerifyOtpAndLoginAsync(
        string phoneNumber,
        string code,
        CancellationToken cancellationToken = default)
    {
        // 1. التحقق من صحة الـ OTP
        var validationResult = await _otpService.ValidateOtpAsync(
            phoneNumber,
            code,
            OtpPurpose.AppointmentBooking,
            cancellationToken);

        if (!validationResult.IsSuccess)
            return Result<PatientAuthResponse>.Failure(validationResult.Errors);

        // 2. البحث عن المريض برقم الهاتف (بما في ذلك المحذوفين منطقياً للتحقق من الحالة)
        var patient = await _context.Patients
            .IgnoreQueryFilters() // للتأكد من حالة IsDeleted
            .FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber, cancellationToken);

        // 3. إذا كان موجوداً ولكن محذوفاً منطقياً -> لا يمكن استخدامه
        if (patient is not null && patient.IsDeleted)
            return Result<PatientAuthResponse>.Failure(UserErrors.AccountDeactivated);

        // 4. إذا لم يكن موجوداً، نقوم بإنشائه
        if (patient is null)
        {
            patient = new Patient
            {
                Id = Guid.CreateVersion7(),
                PhoneNumber = phoneNumber,
                FirstName = "مريض",
                LastName = string.Empty,
                ApplicationUserId = null
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("تم إنشاء مريض جديد برقم {PhoneNumber} أثناء تسجيل الدخول بـ OTP", phoneNumber);
        }

        // 5. توليد JWT قصير المدى خاص بالمريض
        var accessToken = _jwtTokenGenerator.GeneratePatientToken(
            patient.Id,
            patient.PhoneNumber,
            patient.FullName);

        // 6. إرجاع الـ Response
        return Result<PatientAuthResponse>.Success(new PatientAuthResponse(
            PatientId: patient.Id,
            AccessToken: accessToken,
            ExpiresInSeconds: PatientTokenExpirySeconds,
            IsPermanentAccount: patient.IsAccountLinked
        ));
    }

    /// <summary>
    /// إنشاء حساب دائم للمريض (Email + Password) وربطه ببيانات Patient القديمة بعد تأكيد الـ OTP
    /// </summary>
    public async Task<Result<PatientAuthResponse>> RegisterPermanentAccountAsync(
        string email,
        string password,
        string phoneNumber,
        string otpCode,
        CancellationToken cancellationToken = default)
    {
        // 1. التحقق من صحة الـ OTP (بغرض ربط الحساب)
        var validationResult = await _otpService.ValidateOtpAsync(
            phoneNumber,
            otpCode,
            OtpPurpose.LinkAccount,
            cancellationToken);

        if (!validationResult.IsSuccess)
            return Result<PatientAuthResponse>.Failure(validationResult.Errors);

        // 2. البحث عن المريض برقم الهاتف
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber && !p.IsDeleted, cancellationToken);

        if (patient is null)
            return Result<PatientAuthResponse>.Failure(PatientErrors.PhoneNotRegistered);

        // 3. التأكد من أن المريض ليس لديه حساب دائم بالفعل
        if (patient.IsAccountLinked)
            return Result<PatientAuthResponse>.Failure(PatientErrors.AccountAlreadyLinked);

        // 4. التحقق من أن البريد الإلكتروني غير مستخدم من قبل
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser is not null)
            return Result<PatientAuthResponse>.Failure(PatientErrors.EmailAlreadyExists);

        // 5. إنشاء مستخدم Identity جديد
        var user = new ApplicationUser
        {
            Id = Guid.CreateVersion7(),
            UserName = email,
            Email = email,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            MiddleName = patient.MiddleName,
            PhoneNumber = patient.PhoneNumber,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = _dateTime.Now
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            _logger.LogWarning("فشل إنشاء المستخدم للمريض {PhoneNumber}: {Errors}", phoneNumber, errors);
            return Result<PatientAuthResponse>.Failure(PatientErrors.AccountCreationFailed);
        }

        // 6. ربط المستخدم بالمريض
        patient.ApplicationUserId = user.Id;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("تم ربط الحساب الدائم (Email: {Email}) بالمريض (Phone: {PhoneNumber})", email, phoneNumber);

        // 7. توليد JWT للمريض
        var accessToken = _jwtTokenGenerator.GeneratePatientToken(
            patient.Id,
            patient.PhoneNumber,
            patient.FullName);

        return Result<PatientAuthResponse>.Success(new PatientAuthResponse(
            PatientId: patient.Id,
            AccessToken: accessToken,
            ExpiresInSeconds: PatientTokenExpirySeconds,
            IsPermanentAccount: true
        ));
    }

    /// <summary>
    /// تسجيل دخول المريض الذي يمتلك حساباً دائماً باستخدام البريد وكلمة السر
    /// </summary>
    public async Task<Result<PatientAuthResponse>> LoginWithEmailAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        // 1. البحث عن المستخدم في نظام Identity
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<PatientAuthResponse>.Failure(UserErrors.InvalidCredentials);

        // 2. التحقق من صحة كلمة المرور
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordValid)
            return Result<PatientAuthResponse>.Failure(UserErrors.InvalidCredentials);

        // 3. التأكد من أن المستخدم نشط
        if (!user.IsActive)
            return Result<PatientAuthResponse>.Failure(UserErrors.AccountDeactivated);

        // 4. البحث عن المريض المرتبط بهذا المستخدم
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id && !p.IsDeleted, cancellationToken);

        if (patient is null)
        {
            _logger.LogWarning("المستخدم {Email} ليس لديه ملف Patient مرتبط", email);
            return Result<PatientAuthResponse>.Failure(PatientErrors.NotFound);
        }

        // 5. تحديث آخر وقت دخول
        user.LastLoginAt = _dateTime.Now;
        await _userManager.UpdateAsync(user);

        // 6. توليد JWT للمريض
        var accessToken = _jwtTokenGenerator.GeneratePatientToken(
            patient.Id,
            patient.PhoneNumber,
            patient.FullName);

        _logger.LogInformation("تسجيل دخول المريض {Email} بالبريد وكلمة السر", email);

        return Result<PatientAuthResponse>.Success(new PatientAuthResponse(
            PatientId: patient.Id,
            AccessToken: accessToken,
            ExpiresInSeconds: PatientTokenExpirySeconds,
            IsPermanentAccount: true
        ));
    }
}