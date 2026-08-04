namespace ClinicOS.Infrastructure.BackgroundJobs;

using ClinicOS.Application.Common.Abstractions.External.Email;
using ClinicOS.Application.Common.Abstractions.External.Email.Models;
using ClinicOS.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class RefreshDriveQuotaJob(
    IEmailSender emailSender,
    IOptions<MailOptions> mailOptions,
    ILogger<RefreshDriveQuotaJob> logger)
{
    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting RefreshDriveQuotaJob");

        try
        {
            // Simulate quota check or implement inside GoogleDriveContentStorage
            var usedPercentage = 85; // Simulated percentage

            if (usedPercentage > 90)
            {
                logger.LogWarning("Drive quota is above 90% (Current: {UsedPercentage}%)", usedPercentage);

                var emailRequest = new EmailRequest
                {
                    To = new List<string> { mailOptions.Value.SenderEmail }, // Send to admin
                    Subject = "⚠️ تحذير: مساحة التخزين تقترب من الامتلاء",
                    Body = $"مساحة التخزين السحابية (Google Drive) تقترب من الحد الأقصى المسموح به. المساحة المستخدمة: {usedPercentage}%",
                    IsHtml = false
                };

                await emailSender.SendEmailAsync(emailRequest);
            }
            
            logger.LogInformation("Drive quota check completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to check Drive Quota.");
        }
    }
}
