using FluentValidation;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctorAvailabilities;

public sealed class GetDoctorAvailabilitiesQueryValidator : AbstractValidator<GetDoctorAvailabilitiesQuery>
{
    public GetDoctorAvailabilitiesQueryValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("معرف الطبيب (DoctorId) مطلوب.");
    }
}