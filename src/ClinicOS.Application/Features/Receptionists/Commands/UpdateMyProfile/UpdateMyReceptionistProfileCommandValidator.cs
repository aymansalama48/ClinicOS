using FluentValidation;

namespace ClinicOS.Application.Features.Receptionists.Commands.UpdateMyProfile;

public sealed class UpdateMyReceptionistProfileCommandValidator : AbstractValidator<UpdateMyReceptionistProfileCommand>
{
    public UpdateMyReceptionistProfileCommandValidator()
    {
        RuleFor(x => x.SpecializationId).NotEmpty().WithMessage("يجب اختيار التخصص.");
    }
}