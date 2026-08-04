using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;

namespace ClinicOS.Application.Features.Specializations.Commands.RemoveSpecializationSchedule;

[Permission(Permissions.Specializations.Update)]
public sealed record RemoveSpecializationScheduleCommand(
    Guid SpecializationId,
    Guid ScheduleId
) : ICommand<bool>, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => [
        "specializations-list",
        $"specialization-details-{SpecializationId}"
    ];
}