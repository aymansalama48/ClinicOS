using FluentValidation;

namespace ClinicOS.Application.Features.Specializations.Commands.ToggleSpecializationSchedule;

public sealed class ToggleSpecializationScheduleCommandValidator : AbstractValidator<ToggleSpecializationScheduleCommand>
{
    public ToggleSpecializationScheduleCommandValidator()
    {
        RuleFor(x => x.SpecializationId).NotEmpty().WithMessage("معرف التخصص مطلوب.");
        RuleFor(x => x.ScheduleId).NotEmpty().WithMessage("معرف الموعد مطلوب.");
    }
}