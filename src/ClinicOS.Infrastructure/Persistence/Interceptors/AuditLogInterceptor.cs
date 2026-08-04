namespace ClinicOS.Infrastructure.Persistence.Interceptors;

using System.Text.Json;
using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Entities.AuditLogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class AuditLogInterceptor(ICurrentUser currentUser) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ProcessAuditLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ProcessAuditLogs(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ProcessAuditLogs(DbContext? context)
    {
        if (context is null) return;

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is not AuditLog 
                        && e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        if (entries.Count == 0) return;

        var auditLogs = new List<AuditLog>();
        var userId = currentUser.UserId;

        // If UserId is Guid.Empty (e.g. system background jobs), we might want to store null
        Guid? nullableUserId = userId == Guid.Empty ? null : userId;

        foreach (var entry in entries)
        {
            var entityName = entry.Metadata.Name;
            var actionType = entry.State.ToString();
            
            var primaryKey = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
            var entityId = primaryKey?.CurrentValue?.ToString() ?? "Unknown";

            var oldValues = entry.State == EntityState.Added ? null : GetValues(entry, true);
            var newValues = entry.State == EntityState.Deleted ? null : GetValues(entry, false);

            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = nullableUserId,
                EntityName = entityName,
                EntityId = entityId,
                ActionType = actionType,
                OldValues = oldValues,
                NewValues = newValues,
                CreatedAt = DateTime.UtcNow
            };

            auditLogs.Add(auditLog);
        }

        context.Set<AuditLog>().AddRange(auditLogs);
    }

    private static string GetValues(EntityEntry entry, bool isOriginal)
    {
        var values = new Dictionary<string, object?>();

        foreach (var property in entry.Properties)
        {
            if (property.IsTemporary) continue;

            if (isOriginal)
            {
                if (property.Metadata.IsPrimaryKey() || entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                {
                    values[property.Metadata.Name] = property.OriginalValue;
                }
            }
            else
            {
                if (property.Metadata.IsPrimaryKey() || entry.State == EntityState.Modified || entry.State == EntityState.Added)
                {
                    values[property.Metadata.Name] = property.CurrentValue;
                }
            }
        }

        return JsonSerializer.Serialize(values);
    }
}
