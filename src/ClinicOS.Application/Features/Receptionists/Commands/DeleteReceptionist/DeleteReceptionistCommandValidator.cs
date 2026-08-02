using FluentValidation;

namespace ClinicOS.Application.Features.Receptionists.Commands.DeleteReceptionist;

public sealed class DeleteReceptionistCommandValidator : AbstractValidator<DeleteReceptionistCommand>
{
    public DeleteReceptionistCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرف موظف الاستقبال مطلوب.");
    }
}