using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Identity.Providers;
using ClinicOS.Application.Common.Abstractions.Identity.Tokens;
using ClinicOS.Application.Common.Errors.Identity;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Patients;
using ClinicOS.Domain.Enums;
using ClinicOS.Domain.Errors.Patients;
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
    private readonly IEnumerable<IExternalAuthProvider> _externalAuthProviders;

    private const int PatientTokenExpirySeconds = 3600;

    public PatientAuthService(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOtpService otpService,
        IJwtTokenGenerator jwtTokenGenerator,
         IEnumerable<IExternalAuthProvider> externalAuthProviders,
        IDateTime dateTime,
        ILogger<PatientAuthService> logger)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _otpService = otpService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _externalAuthProviders = externalAuthProviders;

        _dateTime = dateTime;
        _logger = logger;
    }

    /// <summary>
    /// التحقق من OTP وإنشاء/جلب المريض وإرجاع JWT قصير المدى
    /// purpose بيتحدد من اللي بينادي (AppointmentBooking / ViewBookings / إلخ)
    /// </summary>
    public async Task<Result<PatientAuthResponse>> VerifyOtpAndLoginAsync(
        string phoneNumber,
        string code,
        OtpPurpose purpose,
        CancellationToken cancellationToken = default)
    {
        // 1. التحقق من صحة الـ OTP بنفس الـ purpose اللي اتبعت بيه أصلاً
        var validationResult = await _otpService.ValidateOtpAsync(
            phoneNumber,
            code,
            purpose,
            cancellationToken);

        if (!validationResult.IsSuccess)
            return Result<PatientAuthResponse>.Failure(validationResult.Errors);

        // 2. البحث عن المريض برقم الهاتف (بما في ذلك المحذوفين منطقياً للتحقق من الحالة)
        var patient = await _context.Patients
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber, cancellationToken);

        if (patient is not null && patient.IsDeleted)
            return Result<PatientAuthResponse>.Failure(UserErrors.AccountDeactivated);

        // 3. لو لسه مش موجود بننشئه — لكن ده منطقي بس في حالة AppointmentBooking
        //    (لو الغرض ViewBookings مثلاً ومفيش Patient خالص، يبقى الرقم غلط أصلاً)
        if (patient is null)
        {
            if (purpose != OtpPurpose.AppointmentBooking)
                return Result<PatientAuthResponse>.Failure(UserErrors.NotFound);

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

        // 4. توليد JWT قصير المدى خاص بالمريض
        var accessToken = _jwtTokenGenerator.GeneratePatientToken(
            patient.Id,
            patient.PhoneNumber,
            patient.FullName);

        return Result<PatientAuthResponse>.Success(new PatientAuthResponse(
            PatientId: patient.Id,
            AccessToken: accessToken,
            ExpiresInSeconds: PatientTokenExpirySeconds,
            IsPermanentAccount: patient.IsAccountLinked
        ));
    }

    public async Task<Result<PatientAuthResponse>> RegisterPermanentAccountAsync(
        string email,
        string password,
        string phoneNumber,
        string otpCode,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _otpService.ValidateOtpAsync(
            phoneNumber,
            otpCode,
            OtpPurpose.LinkAccount,
            cancellationToken);

        if (!validationResult.IsSuccess)
            return Result<PatientAuthResponse>.Failure(validationResult.Errors);

        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber && !p.IsDeleted, cancellationToken);

        if (patient is null)
            return Result<PatientAuthResponse>.Failure(PatientErrors.PhoneNotRegistered);

        if (patient.IsAccountLinked)
            return Result<PatientAuthResponse>.Failure(PatientErrors.AccountAlreadyLinked);

        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser is not null)
            return Result<PatientAuthResponse>.Failure(PatientErrors.EmailAlreadyExists);

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

        patient.ApplicationUserId = user.Id;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("تم ربط الحساب الدائم (Email: {Email}) بالمريض (Phone: {PhoneNumber})", email, phoneNumber);

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

    public async Task<Result<PatientAuthResponse>> LoginWithEmailAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<PatientAuthResponse>.Failure(UserErrors.InvalidCredentials);

        if (!user.IsActive)
            return Result<PatientAuthResponse>.Failure(UserErrors.AccountDeactivated);

        // استخدام SignInManager بدل CheckPasswordAsync المباشرة — عشان يفعّل الـ Lockout
        // (كان قبل كده Brute-force ممكن يحصل من غير أي حد لعدد المحاولات)
        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!signInResult.Succeeded)
        {
            if (signInResult.IsLockedOut)
                return Result<PatientAuthResponse>.Failure(UserErrors.AccountLocked);
            return Result<PatientAuthResponse>.Failure(UserErrors.InvalidCredentials);
        }

        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id && !p.IsDeleted, cancellationToken);

        if (patient is null)
        {
            _logger.LogWarning("المستخدم {Email} ليس لديه ملف Patient مرتبط", email);
            return Result<PatientAuthResponse>.Failure(PatientErrors.NotFound);
        }

        user.LastLoginAt = _dateTime.Now;
        await _userManager.UpdateAsync(user);

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

    /// <summary>
    /// تسجيل دخول/إنشاء حساب مريض بجوجل — يعمل الحساب تلقائيًا لو مش موجود
    /// (بعكس الـ Staff، لأن أي حد يقدر أصلًا يعمل حساب مريض دائم بنفسه)
    /// </summary>
    public async Task<Result<PatientAuthResponse>> LoginWithGoogleAsync(
        string idToken,
        CancellationToken cancellationToken = default)
    {
        var provider = _externalAuthProviders.FirstOrDefault(p => p.ProviderName == "Google");
        if (provider is null)
            return Result<PatientAuthResponse>.Failure(ExternalAuthErrors.InvalidToken);

        var tokenResult = await provider.ValidateTokenAsync(idToken, cancellationToken);
        if (!tokenResult.IsSuccess)
            return Result<PatientAuthResponse>.Failure(tokenResult.Errors);

        var externalUser = tokenResult.Data!;

        var user = await _userManager.FindByEmailAsync(externalUser.Email);

        if (user is not null)
        {
            // فيه حساب بالفعل بنفس الإيميل — لازم يكون حساب مريض، مش حساب Staff
            var linkedPatient = await _context.Patients
                .FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id && !p.IsDeleted, cancellationToken);

            if (linkedPatient is null)
                return Result<PatientAuthResponse>.Failure(UserErrors.InvalidCredentials); // إيميل ده لحساب Staff مش مريض

            if (!user.IsActive)
                return Result<PatientAuthResponse>.Failure(UserErrors.AccountDeactivated);

            user.LastLoginAt = _dateTime.Now;
            await _userManager.UpdateAsync(user);

            var accessToken = _jwtTokenGenerator.GeneratePatientToken(
                linkedPatient.Id, linkedPatient.PhoneNumber, linkedPatient.FullName);

            return Result<PatientAuthResponse>.Success(new PatientAuthResponse(
                PatientId: linkedPatient.Id,
                AccessToken: accessToken,
                ExpiresInSeconds: PatientTokenExpirySeconds,
                IsPermanentAccount: true,
                RequiresPhoneNumber: string.IsNullOrWhiteSpace(linkedPatient.PhoneNumber)));
        }

        // مفيش حساب خالص — ننشئ ApplicationUser + Patient جديدين (بدون رقم موبايل)
        var nameParts = externalUser.FullName.Trim().Split(' ', 2);

        var newUser = new ApplicationUser
        {
            Id = Guid.CreateVersion7(),
            UserName = externalUser.Email,
            Email = externalUser.Email,
            FirstName = nameParts[0],
            LastName = nameParts.Length > 1 ? nameParts[1] : string.Empty,
            AvatarUrl = externalUser.AvatarUrl,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = _dateTime.Now
        };

        var createResult = await _userManager.CreateAsync(newUser);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            _logger.LogWarning("فشل إنشاء حساب Google للمريض {Email}: {Errors}", externalUser.Email, errors);
            return Result<PatientAuthResponse>.Failure(PatientErrors.AccountCreationFailed);
        }

        var newPatient = new Patient
        {
            Id = Guid.CreateVersion7(),
            PhoneNumber = string.Empty,   // 👈 لسه مفيش رقم — لازم يكمله بعدين
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            ApplicationUserId = newUser.Id
        };

        _context.Patients.Add(newPatient);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("تم إنشاء مريض جديد بجوجل (Email: {Email})", externalUser.Email);

        var newAccessToken = _jwtTokenGenerator.GeneratePatientToken(
            newPatient.Id, newPatient.PhoneNumber, newPatient.FullName);

        return Result<PatientAuthResponse>.Success(new PatientAuthResponse(
            PatientId: newPatient.Id,
            AccessToken: newAccessToken,
            ExpiresInSeconds: PatientTokenExpirySeconds,
            IsPermanentAccount: true,
            RequiresPhoneNumber: true));
    }
}