using FluentValidation;

namespace ClinicOS.Application.Features.Accounts.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("البريد الإلكتروني مطلوب.")
            .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("رمز إعادة التعيين مطلوب.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("كلمة المرور الجديدة مطلوبة.")
            .MinimumLength(8).WithMessage("كلمة المرور يجب أن لا تقل عن 8 أحرف.")
            .Matches(@"[A-Z]").WithMessage("كلمة المرور يجب أن تحتوي على حرف كبير واحد على الأقل.")
            .Matches(@"[a-z]").WithMessage("كلمة المرور يجب أن تحتوي على حرف صغير واحد على الأقل.")
            .Matches(@"[0-9]").WithMessage("كلمة المرور يجب أن تحتوي على رقم واحد على الأقل.")
            .Matches(@"[\W_]").WithMessage("كلمة المرور يجب أن تحتوي على رمز خاص واحد على الأقل.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("تأكيد كلمة المرور مطلوب.")
            .Equal(x => x.NewPassword).WithMessage("كلمة المرور وتأكيدها غير متطابقين.");
    }
}