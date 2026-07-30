using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Domain.Common.Events;
using ClinicOS.Infrastructure.Persistence.Data;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicOS.Infrastructure.BackgroundJobs;

public class ProcessOutboxMessagesJob
{
    private readonly AppDbContext _dbContext;
    private readonly IPublisher _publisher;
    private readonly IDateTime _dateTime;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;
    private const int MaxRetryCount = 3;

    public ProcessOutboxMessagesJob(
        AppDbContext dbContext,
        IPublisher publisher,
        IDateTime dateTime,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _dbContext = dbContext;
        _publisher = publisher;
        _dateTime = dateTime;
        _logger = logger;
    }

    // دالة المعالجة التي سينفذها Hangfire
    [AutomaticRetry(Attempts = 0)] // نلغي إعادة المحاولة التلقائية لـ Hangfire هنا لأننا ندير الـ RetryCount بجدول الـ Outbox
    public async Task ProcessAsync()
    {
        // 1. سحب الرسائل المعلقة
        var messages = await _dbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null && m.RetryCount < MaxRetryCount)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync();

        if (!messages.Any()) return;

        foreach (var message in messages)
        {
            try
            {
                // 2. فك التشفير للحدث الأصلي
                var eventType = Type.GetType(message.Type);
                if (eventType is null)
                {
                    _logger.LogError("تعذر التعرف على نوع الحدث: {Type}", message.Type);
                    message.Error = $"Type {message.Type} missing.";
                    message.RetryCount = MaxRetryCount;
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
                await _publisher.Publish(domainEvent);

                // 4. تحديث حالة الرسالة
                message.ProcessedOnUtc = _dateTime.Now;
                message.Error = null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء معالجة Outbox Message ID: {Id}", message.Id);
                message.RetryCount++;
                message.Error = ex.ToString();
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}