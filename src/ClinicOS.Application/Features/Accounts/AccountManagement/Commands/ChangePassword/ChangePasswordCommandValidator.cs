using FluentValidation;

namespace ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ChangePassword;

/// <summary>
/// التحقق من صحة وقواعد بيانات تغيير كلمة المرور
/// </summary>
public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        // 1. التحقق من كلمة المرور الحالية
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("كلمة المرور الحالية مطلوبة.");

        // 2. التحقق من كلمة المرور الجديدة وقواعد الأمان
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("كلمة المرور الجديدة مطلوبة.")
            .MinimumLength(8).WithMessage("يجب أن لا تقل كلمة المرور عن 8 أحرف.")
            .Matches(@"[A-Z]").WithMessage("يجب أن تحتوي كلمة المرور على حرف كبير واحد على الأقل.")
            .Matches(@"[a-z]").WithMessage("يجب أن تحتوي كلمة المرور على حرف صغير واحد على الأقل.")
            .Matches(@"[0-9]").WithMessage("يجب أن تحتوي كلمة المرور على رقم واحد على الأقل.")
            .Matches(@"[\W_]").WithMessage("يجب أن تحتوي كلمة المرور على رمز خاص واحد على الأقل.")
            .NotEqual(x => x.CurrentPassword).WithMessage("يجب أن تكون كلمة المرور الجديدة مختلفة عن كلمة المرور الحالية.");

        // 3. التحقق من تأكيد كلمة المرور الجديدة
        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("تأكيد كلمة المرور الجديدة مطلوب.")
            .Equal(x => x.NewPassword).WithMessage("كلمة المرور الجديدة وتأكيدها غير متطابقين.");
    }
}