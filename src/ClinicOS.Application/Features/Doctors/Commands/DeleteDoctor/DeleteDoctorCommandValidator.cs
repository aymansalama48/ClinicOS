using FluentValidation;

namespace ClinicOS.Application.Features.Doctors.Commands.DeleteDoctor;

public sealed class DeleteDoctorCommandValidator : AbstractValidator<DeleteDoctorCommand>
{
    public DeleteDoctorCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرف الطبيب مطلوب.");
    }
}