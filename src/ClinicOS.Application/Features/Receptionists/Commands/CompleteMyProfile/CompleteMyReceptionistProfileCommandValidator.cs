using FluentValidation;

namespace ClinicOS.Application.Features.Receptionists.Commands.CompleteMyProfile;

public sealed class CompleteMyReceptionistProfileCommandValidator : AbstractValidator<CompleteMyReceptionistProfileCommand>
{
    public CompleteMyReceptionistProfileCommandValidator()
    {
        RuleFor(x => x.SpecializationId).NotEmpty().WithMessage("يجب اختيار التخصص.");
    }
}