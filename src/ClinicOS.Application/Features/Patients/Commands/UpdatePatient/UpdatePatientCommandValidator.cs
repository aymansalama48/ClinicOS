using FluentValidation;

namespace ClinicOS.Application.Features.Patients.Commands.UpdatePatient;

public sealed class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
{
    public UpdatePatientCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty().WithMessage("معرف المريض مطلوب.");
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("الاسم الأول مطلوب.");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("اسم العائلة مطلوب.");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("رقم الهاتف مطلوب.");
    }
}