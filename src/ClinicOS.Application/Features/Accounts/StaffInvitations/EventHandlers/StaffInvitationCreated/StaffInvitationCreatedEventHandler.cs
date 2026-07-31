namespace ClinicOS.Application.Features.Accounts.StaffInvitations.EventHandlers.StaffInvitationCreated;

using System;
using System.Threading;
using System.Threading.Tasks;

using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Events;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.IdentityTemplats;
using ClinicOS.Domain.Entities.Invitation.Events;

using MediatR;

public sealed class StaffInvitationCreatedEventHandler(IJobScheduler jobScheduler)
    : INotificationHandler<DomainEventNotification<StaffInvitationCreatedEvent>>
{
    public Task Handle(
        DomainEventNotification<StaffInvitationCreatedEvent> notification,
        CancellationToken cancellationToken)
    {
        var invitation = notification.DomainEvent.Invitation;

        // حساب عدد الساعات المتبقية للصلاحية بناءً على الكيان الأصلي
        var expiryHours = (int)Math.Max(1, (invitation.ExpiresAtUtc - DateTime.UtcNow).TotalHours);

        // تجهيز الموديل بما يطابق جدولك تماماً
        var templateModel = new StaffInvitationTemplateModel
        {
            InvitedEmail = invitation.Email,
            AdminName = invitation.AdminName,
            Token = invitation.Token, // السيرفس ستقوم بتحويله لرابط كامل
            RoleName = TranslateRole(invitation.Role),
            ExpiryHours = expiryHours
        };

        // رمي المَهمة لـ Hangfire باستخدام الخدمة الموحدة للإشعارات
        jobScheduler.Enqueue<IIdentityNotificationService>(notificationService =>
            notificationService.SendStaffInvitationEmailAsync(
                invitation.Email,
                templateModel));

        return Task.CompletedTask;
    }

    // دالة مساعدة لترجمة الـ Roles لتظهر في الإيميل بشكل جميل
    private static string TranslateRole(string role) => role switch
    {
        "Doctor" => "طبيب",
        "Receptionist" => "موظف استقبال",
        "Admin" => "مدير نظام",
        _ => "موظف"
    };
}