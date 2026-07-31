namespace ClinicOS.Application.Features.Accounts.PatientAuth.Commands.PatientOtpLogin;

using FluentValidation;

public sealed class PatientOtpLoginCommandValidator : AbstractValidator<PatientOtpLoginCommand>
{
    public PatientOtpLoginCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("رقم الهاتف مطلوب");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("رمز التحقق (OTP) مطلوب")
            .Length(4, 6).WithMessage("رمز التحقق يجب أن يكون بين 4 و 6 أرقام");

        RuleFor(x => x.Purpose)
            .IsInEnum().WithMessage("الغرض من الـ OTP غير صالح");
    }
}