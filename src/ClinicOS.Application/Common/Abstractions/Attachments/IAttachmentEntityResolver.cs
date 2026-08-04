using System;

namespace ClinicOS.Application.Common.Abstractions.Attachments;

public interface IAttachmentEntityResolver
{
    bool TryResolve(string entityType, out ResolvedEntity resolvedEntity);
    Type? GetEntityType(string entityType);
}
