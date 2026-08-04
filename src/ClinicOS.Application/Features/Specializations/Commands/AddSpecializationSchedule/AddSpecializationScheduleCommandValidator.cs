using FluentValidation;

namespace ClinicOS.Application.Features.Specializations.Commands.AddSpecializationSchedule;

public sealed class AddSpecializationScheduleCommandValidator : AbstractValidator<AddSpecializationScheduleCommand>
{
    public AddSpecializationScheduleCommandValidator()
    {
        RuleFor(x => x.SpecializationId).NotEmpty().WithMessage("معرف التخصص مطلوب.");
        RuleFor(x => x.DayOfWeek).IsInEnum().WithMessage("يوم الأسبوع غير صالح.");
        RuleFor(x => x.Period).IsInEnum().WithMessage("الفترة غير صالحة.");
        RuleFor(x => x.StartTime).NotEmpty().WithMessage("وقت البداية مطلوب.");
        RuleFor(x => x.EndTime).NotEmpty().WithMessage("وقت النهاية مطلوب.");

        RuleFor(x => x)
            .Must(x => x.StartTime != x.EndTime)
            .WithMessage("وقت البداية ووقت النهاية لا يمكن أن يكونا متطابقين.");
    }
}