namespace ClinicOS.Application.Features.Accounts.PatientAuth.Commands.PatientEmailLogin;

using FluentValidation;

public sealed class PatientEmailLoginCommandValidator : AbstractValidator<PatientEmailLoginCommand>
{
    public PatientEmailLoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("البريد الإلكتروني مطلوب")
            .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("كلمة المرور مطلوبة");
    }
}