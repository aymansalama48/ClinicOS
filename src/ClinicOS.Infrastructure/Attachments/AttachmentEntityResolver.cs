namespace ClinicOS.Infrastructure.Attachments;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ClinicOS.Application.Common.Abstractions.Attachments;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Domain.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public sealed class AttachmentEntityResolver : IAttachmentEntityResolver
{
    private readonly Dictionary<string, ResolvedEntity> _entities;

    public AttachmentEntityResolver(IServiceProvider serviceProvider)
    {
        _entities = new Dictionary<string, ResolvedEntity>(StringComparer.OrdinalIgnoreCase);

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        if (dbContext is DbContext efContext)
        {
            var entityTypes = efContext.Model.GetEntityTypes()
                .Where(e => typeof(IHasAttachments).IsAssignableFrom(e.ClrType));

            foreach (var et in entityTypes)
            {
                var tableName = et.GetTableName();
                if (tableName != null)
                {
                    // Sanitize table name for folder usage
                    var sanitized = SanitizeForFolderName(tableName);
                    _entities[et.ClrType.Name] = new ResolvedEntity(sanitized, et.ClrType);
                }
            }
        }
    }

    public bool TryResolve(string entityType, out ResolvedEntity resolvedEntity)
    {
        if (string.IsNullOrWhiteSpace(entityType))
        {
            resolvedEntity = default!;
            return false;
        }

        return _entities.TryGetValue(entityType, out resolvedEntity!);
    }

    public Type? GetEntityType(string entityType)
    {
        return TryResolve(entityType, out var resolved) ? resolved.EntityType : null;
    }

    private static string SanitizeForFolderName(string name)
    {
        var sanitized = Regex.Replace(name, @"[^a-zA-Z0-9_-]", "");
        return sanitized.ToLowerInvariant();
    }
}
