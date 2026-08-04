using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;

namespace ClinicOS.Application.Features.Specializations.Commands.UpdateSpecialization;

[Permission(Permissions.Specializations.Update)]
public sealed record UpdateSpecializationCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid? IconAttachmentId
) : ICommand<bool>, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => [
        "specializations-list",
        $"specialization-details-{Id}"
    ];
}