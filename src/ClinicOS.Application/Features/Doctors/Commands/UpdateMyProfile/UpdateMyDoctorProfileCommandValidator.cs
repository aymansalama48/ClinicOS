using FluentValidation;

namespace ClinicOS.Application.Features.Doctors.Commands.UpdateMyProfile;

public sealed class UpdateMyDoctorProfileCommandValidator : AbstractValidator<UpdateMyDoctorProfileCommand>
{
    public UpdateMyDoctorProfileCommandValidator()
    {
        RuleFor(x => x.SpecializationId).NotEmpty().WithMessage("يجب اختيار التخصص.");
        RuleFor(x => x.ConsultationFee).GreaterThanOrEqualTo(0).WithMessage("سعر الكشف لا يمكن أن يكون بالسالب.");
        RuleFor(x => x.UrgentSurchargeFee).GreaterThanOrEqualTo(0).WithMessage("رسوم الكشف العاجل لا يمكن أن تكون بالسالب.");
    }
}