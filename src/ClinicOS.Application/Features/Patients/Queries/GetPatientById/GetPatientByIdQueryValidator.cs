using FluentValidation;

namespace ClinicOS.Application.Features.Patients.Queries.GetPatientById;

public sealed class GetPatientByIdQueryValidator : AbstractValidator<GetPatientByIdQuery>
{
    public GetPatientByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرف المريض مطلوب.");
    }
}