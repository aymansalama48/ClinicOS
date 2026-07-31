namespace ClinicOS.Application.Features.Accounts.StaffInvitations.Commands.SendInvitation;

using ClinicOS.Domain.Constants;
using FluentValidation;

public sealed class SendStaffInvitationCommandValidator : AbstractValidator<SendStaffInvitationCommand>
{
    public SendStaffInvitationCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("البريد الإلكتروني مطلوب.")
            .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("الدور الوظيفي مطلوب.")
            .Must(role => role == Roles.Admin || role == Roles.Doctor || role == Roles.Receptionist)
            .WithMessage("الدور الوظيفي غير صالح.");

        RuleFor(x => x.SpecializationId)
            .NotEmpty().When(x => x.Role == Roles.Doctor)
            .WithMessage("يجب تحديد التخصص عند إضافة طبيب.");
    }
}