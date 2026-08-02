using FluentValidation;

namespace ClinicOS.Application.Features.Specializations.Queries.GetSpecializationById;

public sealed class GetSpecializationByIdQueryValidator : AbstractValidator<GetSpecializationByIdQuery>
{
    public GetSpecializationByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرف التخصص مطلوب.");
    }
}