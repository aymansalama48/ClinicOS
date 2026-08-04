using FluentValidation;

namespace ClinicOS.Application.Features.Patients.Queries.GetPatients;

public sealed class GetPatientsQueryValidator : AbstractValidator<GetPatientsQuery>
{
    public GetPatientsQueryValidator()
    {
        RuleFor(x => x.Parameters.PageNumber)
            .GreaterThan(0).WithMessage("رقم الصفحة يجب أن يكون أكبر من الصفر.");

        RuleFor(x => x.Parameters.PageSize)
            .GreaterThan(0).WithMessage("حجم الصفحة يجب أن يكون أكبر من الصفر.")
            .LessThanOrEqualTo(100).WithMessage("الحد الأقصى لحجم الصفحة هو 100.");
    }
}