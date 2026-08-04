namespace ClinicOS.Infrastructure.BackgroundJobs;

using ClinicOS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class CleanupExpiredTokensJob(
    AppDbContext dbContext,
    ILogger<CleanupExpiredTokensJob> logger)
{
    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting CleanupExpiredTokensJob");

        var now = DateTime.UtcNow;

        // Cleanup expired refresh tokens
        var expiredTokens = await dbContext.RefreshTokens
            .Where(rt => rt.ExpiryDate < now)
            .ExecuteDeleteAsync(cancellationToken);

        logger.LogInformation("Deleted {Count} expired refresh tokens.", expiredTokens);

        // Cleanup expired OTPs
        var expiredOtps = await dbContext.OtpVerifications
            .Where(otp => otp.Expiry < now)
            .ExecuteDeleteAsync(cancellationToken);

        logger.LogInformation("Deleted {Count} expired OTPs.", expiredOtps);

        // Cleanup expired staff invitations
        var expiredInvitations = await dbContext.StaffInvitations
            .Where(inv => inv.ExpiresAtUtc < now)
            .ExecuteDeleteAsync(cancellationToken);

        logger.LogInformation("Deleted {Count} expired invitations.", expiredInvitations);
    }
}
