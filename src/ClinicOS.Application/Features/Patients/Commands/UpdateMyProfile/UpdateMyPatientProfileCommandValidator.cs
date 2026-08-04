using FluentValidation;

namespace ClinicOS.Application.Features.Patients.Commands.UpdateMyProfile;

public sealed class UpdateMyPatientProfileCommandValidator : AbstractValidator<UpdateMyPatientProfileCommand>
{
    public UpdateMyPatientProfileCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("الاسم الأول مطلوب.");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("اسم العائلة مطلوب.");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("رقم الهاتف مطلوب.");
    }
}