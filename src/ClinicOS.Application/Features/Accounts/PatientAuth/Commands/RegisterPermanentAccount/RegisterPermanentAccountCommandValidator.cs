namespace ClinicOS.Application.Features.Accounts.PatientAuth.Commands.RegisterPermanentAccount;

using ClinicOS.Application.Common.Validation; // 👈 استدعاء الـ Extension
using FluentValidation;

public sealed class RegisterPermanentAccountCommandValidator : AbstractValidator<RegisterPermanentAccountCommand>
{
    public RegisterPermanentAccountCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("البريد الإلكتروني مطلوب")
            .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة");

        // 👇 استخدام السطر الموحد بدلاً من كتابة الرولز يدوياً
        RuleFor(x => x.Password)
            .ApplyStandardPasswordRules();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("رقم الهاتف مطلوب");

        RuleFor(x => x.OtpCode)
            .NotEmpty().WithMessage("رمز التحقق (OTP) مطلوب");
    }
}