namespace ClinicOS.Infrastructure.BackgroundJobs;

using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.AppointmentTemplats;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Domain.Enums;
using ClinicOS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class AppointmentReminderJob(
    AppDbContext dbContext,
    IAppointmentNotificationService appointmentNotificationService,
    ILogger<AppointmentReminderJob> logger)
{
    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting AppointmentReminderJob");

        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        var upcomingAppointments = await dbContext.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.AppointmentDate == tomorrow && a.Status == AppointmentStatus.Confirmed)
            .ToListAsync(cancellationToken);

        foreach (var appointment in upcomingAppointments)
        {
            try
            {
                var doctorUser = await dbContext.Users.FindAsync(new object[] { appointment.Doctor.ApplicationUserId }, cancellationToken);
                var doctorName = doctorUser != null ? $"د. {doctorUser.FirstName} {doctorUser.LastName}" : "طبيبك المعالج";

                var patientUser = appointment.Patient.ApplicationUserId.HasValue 
                    ? await dbContext.Users.FindAsync(new object[] { appointment.Patient.ApplicationUserId.Value }, cancellationToken) 
                    : null;
                
                var patientEmail = patientUser?.Email;

                if (!string.IsNullOrEmpty(patientEmail))
                {
                    var model = new AppointmentReminderTemplateModel
                    {
                        PatientName = appointment.Patient.FirstName,
                        DoctorName = doctorName,
                        AppointmentDate = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                        AppointmentTime = appointment.Period.ToString(),
                        PhoneNumber = appointment.Patient.PhoneNumber
                    };

                    await appointmentNotificationService.SendAppointmentReminderAsync(patientEmail, model);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send reminder for appointment {AppointmentId}", appointment.Id);
            }
        }

        logger.LogInformation("Sent reminders for {Count} appointments.", upcomingAppointments.Count);
    }
}
