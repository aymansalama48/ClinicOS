using FluentValidation;

namespace ClinicOS.Application.Features.Specializations.Commands.ToggleSpecializationStatus;

public sealed class ToggleSpecializationStatusCommandValidator : AbstractValidator<ToggleSpecializationStatusCommand>
{
    public ToggleSpecializationStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرف التخصص مطلوب.");
    }
}