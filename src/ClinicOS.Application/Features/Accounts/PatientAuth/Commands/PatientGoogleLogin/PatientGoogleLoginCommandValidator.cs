namespace ClinicOS.Application.Features.Accounts.PatientAuth.Commands.PatientGoogleLogin;

using FluentValidation;

public sealed class PatientGoogleLoginCommandValidator : AbstractValidator<PatientGoogleLoginCommand>
{
    public PatientGoogleLoginCommandValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("رمز Google (IdToken) مطلوب");
    }
}