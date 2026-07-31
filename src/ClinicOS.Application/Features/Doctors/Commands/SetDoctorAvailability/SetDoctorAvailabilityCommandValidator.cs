using FluentValidation;

namespace ClinicOS.Application.Features.Doctors.Commands.SetDoctorAvailability;

public sealed class SetDoctorAvailabilityCommandValidator : AbstractValidator<SetDoctorAvailabilityCommand>
{
    public SetDoctorAvailabilityCommandValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("معرف الطبيب (DoctorId) مطلوب.");

        RuleFor(x => x.DayOfWeek)
            .IsInEnum().WithMessage("يوم الأسبوع غير صحيح.");

        RuleFor(x => x.Period)
            .IsInEnum().WithMessage("الفترة المحددة غير صحيحة.");

        // التحقق إن النهاية بعد البداية
        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime).WithMessage("وقت النهاية يجب أن يكون بعد وقت البداية.");

        // التحقق من الحد الأقصى للمرضى لو كان مبعوت (مش Null)
        RuleFor(x => x.MaxPatients)
            .GreaterThan(0).When(x => x.MaxPatients.HasValue)
            .WithMessage("الحد الأقصى للمرضى يجب أن يكون رقماً أكبر من الصفر.");
    }
}