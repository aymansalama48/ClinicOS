using FluentValidation;

namespace ClinicOS.Application.Features.Receptionists.Queries.GetReceptionistById;

public sealed class GetReceptionistByIdQueryValidator : AbstractValidator<GetReceptionistByIdQuery>
{
    public GetReceptionistByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("معرف موظف الاستقبال مطلوب.");
    }
}