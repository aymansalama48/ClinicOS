namespace ClinicOS.Application.Features.Accounts.StaffInvitations.Commands.AcceptInvitation;

using ClinicOS.Application.Common.Validation; // 👈 استدعاء الـ Extension
using FluentValidation;

public sealed class AcceptInvitationCommandValidator : AbstractValidator<AcceptInvitationCommand>
{
    public AcceptInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationToken)
            .NotEmpty().WithMessage("رمز الدعوة مطلوب.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("الاسم الكامل مطلوب.")
            .MinimumLength(3).WithMessage("الاسم الكامل يجب أن يكون 3 أحرف على الأقل.");

        // 👇 استخدام السطر الموحد لضمان التطابق التام مع Identity
        RuleFor(x => x.Password)
            .ApplyStandardPasswordRules();

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("رقم الهاتف طويل جداً.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
    }
}