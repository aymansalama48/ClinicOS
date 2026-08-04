using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;

namespace ClinicOS.Application.Features.Specializations.Commands.ToggleSpecializationStatus;

[Permission(Permissions.Specializations.Update)]
public sealed record ToggleSpecializationStatusCommand(Guid Id) : ICommand<bool>, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => [
        "specializations-list",
        $"specialization-details-{Id}"
    ];
}