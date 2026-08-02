using FluentValidation;

namespace ClinicOS.Application.Features.Doctors.Commands.CompleteMyProfile;

public sealed class CompleteMyDoctorProfileCommandValidator : AbstractValidator<CompleteMyDoctorProfileCommand>
{
    public CompleteMyDoctorProfileCommandValidator()
    {
        RuleFor(x => x.SpecializationId).NotEmpty().WithMessage("يجب اختيار التخصص.");
        RuleFor(x => x.ConsultationFee).GreaterThanOrEqualTo(0).WithMessage("سعر الكشف لا يمكن أن يكون بالسالب.");
        RuleFor(x => x.UrgentSurchargeFee).GreaterThanOrEqualTo(0).WithMessage("رسوم الكشف العاجل لا يمكن أن تكون بالسالب.");
        RuleFor(x => x.YearsOfExperience).GreaterThanOrEqualTo(0).When(x => x.YearsOfExperience.HasValue).WithMessage("سنوات الخبرة غير صالحة.");
    }
}