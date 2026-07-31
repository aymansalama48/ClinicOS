namespace ClinicOS.Application.Features.Accounts.StaffAuth.Commands.StaffGoogleLogin;

using FluentValidation;

public sealed class StaffGoogleLoginCommandValidator : AbstractValidator<StaffGoogleLoginCommand>
{
    public StaffGoogleLoginCommandValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("رمز Google (IdToken) مطلوب");
    }
}