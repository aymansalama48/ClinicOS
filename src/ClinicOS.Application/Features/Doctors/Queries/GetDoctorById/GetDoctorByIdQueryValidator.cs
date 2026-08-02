using FluentValidation;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctorById;

public sealed class GetDoctorByIdQueryValidator : AbstractValidator<GetDoctorByIdQuery>
{
    public GetDoctorByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("معرف الطبيب مطلوب.");
    }
}