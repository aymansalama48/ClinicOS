using FluentValidation;

namespace ClinicOS.Application.Features.Specializations.Commands.DeleteSpecialization;

public sealed class DeleteSpecializationCommandValidator : AbstractValidator<DeleteSpecializationCommand>
{
    public DeleteSpecializationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرف التخصص مطلوب.");
    }
}