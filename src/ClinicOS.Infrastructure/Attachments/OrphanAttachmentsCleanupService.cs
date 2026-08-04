namespace ClinicOS.Infrastructure.Attachments;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClinicOS.Application.Common.Abstractions.External.FileStorage;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class OrphanAttachmentsCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrphanAttachmentsCleanupService> _logger;

    public OrphanAttachmentsCleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<OrphanAttachmentsCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Orphan Attachments Cleanup Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupOrphansAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing Orphan Attachments Cleanup.");
            }

            // تعمل كل 24 ساعة
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task CleanupOrphansAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var fileStorage = scope.ServiceProvider.GetRequiredService<IFileStorage>();

        // الملفات المنشأة منذ أكثر من 24 ساعة
        var cutoffDate = DateTime.UtcNow.AddHours(-24);

        var orphans = await context.Attachments
            .Where(a => a.EntityId == null && a.CreatedAt < cutoffDate)
            .ToListAsync(cancellationToken);

        if (!orphans.Any())
            return;

        _logger.LogInformation("Found {Count} orphan attachments to delete.", orphans.Count);

        foreach (var orphan in orphans)
        {
            try
            {
                // محاولة الحذف من مساحة التخزين الفعلية
                var deleteResult = await fileStorage.DeleteAsync(orphan.FileId, cancellationToken);
                
                if (deleteResult.IsSuccess)
                {
                    // إذا نجح الحذف أو كان غير موجود، يتم حذفه من الـ DB
                    context.Attachments.Remove(orphan);
                }
                else
                {
                    _logger.LogWarning("Failed to delete physical file for attachment {Id}", orphan.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting orphan attachment {Id}", orphan.Id);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
