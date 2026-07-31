using FluentValidation;

namespace ClinicOS.Application.Features.Accounts.Authentication.Commands.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("الـ Refresh Token مطلوب.");
    }
}