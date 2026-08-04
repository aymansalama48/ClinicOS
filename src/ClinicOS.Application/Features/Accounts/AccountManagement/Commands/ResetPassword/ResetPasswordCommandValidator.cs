using ClinicOS.Application.Common.Validation;
using FluentValidation;

namespace ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("البريد الإلكتروني مطلوب.")
            .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("رمز إعادة التعيين مطلوب.");

        // استخدام قواعد كلمة المرور الموحدة
        RuleFor(x => x.NewPassword)
            .ApplyStandardPasswordRules();

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("تأكيد كلمة المرور مطلوب.")
            .Equal(x => x.NewPassword).WithMessage("كلمة المرور وتأكيدها غير متطابقين.");
    }
}
