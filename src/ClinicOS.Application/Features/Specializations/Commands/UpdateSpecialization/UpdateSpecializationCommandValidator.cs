using FluentValidation;

namespace ClinicOS.Application.Features.Specializations.Commands.UpdateSpecialization;

public sealed class UpdateSpecializationCommandValidator : AbstractValidator<UpdateSpecializationCommand>
{
    public UpdateSpecializationCommandValidator()
    {
        RuleFor(x => x.SpecializationId).NotEmpty().WithMessage("معرف التخصص مطلوب.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("اسم التخصص مطلوب.");
    }
}