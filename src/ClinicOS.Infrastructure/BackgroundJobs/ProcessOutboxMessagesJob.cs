using System.Text.Json;
using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Domain.Common.Events;
using ClinicOS.Infrastructure.Persistence.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClinicOS.Infrastructure.BackgroundJobs;

public class ProcessOutboxMessagesJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;
    private const int MaxRetryCount = 3;

    public ProcessOutboxMessagesJob(
        IServiceScopeFactory scopeFactory,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
                var dateTime = scope.ServiceProvider.GetRequiredService<IDateTime>();

                // 1. سحب الرسائل المعلقة التي لم تتجاوز حد المحاولات المسموح
                var messages = await dbContext.OutboxMessages
                    .Where(m => m.ProcessedOnUtc == null && m.RetryCount < MaxRetryCount)
                    .OrderBy(m => m.OccurredOnUtc)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        // 2. فك التشفير للحدث الأصلي (Deserialization)
                        var eventType = Type.GetType(message.Type);
                        if (eventType is null)
                        {
                            _logger.LogError("تعذر التعرف على نوع الحدث: {Type}", message.Type);
                            message.Error = $"Type {message.Type} missing.";
                            message.RetryCount = MaxRetryCount; // إيقاف الرسالة فوراً
                            continue;
                        }

                        var domainEvent = JsonSerializer.Deserialize(message.Content, eventType) as IDomainEvent;
                        if (domainEvent is null)
                        {
                            _logger.LogError("فشل فك تشفير محتوى الحدث للرسالة ذات المعرف: {Id}", message.Id);
                            message.Error = "Failed to deserialize event content.";
                            message.RetryCount = MaxRetryCount;
                            continue;
                        }

                        // 3. نشر الحدث داخل التطبيق عبر MediatR
                        await publisher.Publish(domainEvent, stoppingToken);

                        // 4. تحديث حالة الرسالة باستخدام واجهة الوقت الموحدة للنظام
                        message.ProcessedOnUtc = dateTime.Now;
                        message.Error = null;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "خطأ أثناء معالجة Outbox Message ID: {Id}", message.Id);

                        message.RetryCount++;
                        message.Error = ex.ToString();
                    }
                }

                if (messages.Count > 0)
                {
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ غير متوقع في Outbox Background Processor");
            }

            // التأخير بين كل فحص والآخر (5 ثوانٍ)
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}