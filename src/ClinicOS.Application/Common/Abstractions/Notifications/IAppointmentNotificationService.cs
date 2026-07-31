using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.AppointmentTemplats;

namespace ClinicOS.Application.Common.Abstractions.Notifications;

public interface IAppointmentNotificationService
{
    Task SendAppointmentConfirmationAsync(string email, AppointmentConfirmationTemplateModel model);
    Task SendAppointmentReminderAsync(string email, AppointmentReminderTemplateModel model);
}