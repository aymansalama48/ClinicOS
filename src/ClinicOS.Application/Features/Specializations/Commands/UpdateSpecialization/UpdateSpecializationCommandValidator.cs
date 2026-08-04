using FluentValidation;

namespace ClinicOS.Application.Features.Specializations.Commands.UpdateSpecialization;

public sealed class UpdateSpecializationCommandValidator : AbstractValidator<UpdateSpecializationCommand>
{
    public UpdateSpecializationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرف التخصص مطلوب.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم التخصص مطلوب.")
            .MaximumLength(100).WithMessage("اسم التخصص يجب ألا يتجاوز 100 حرف.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("الوصف يجب ألا يتجاوز 500 حرف.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}