using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;
using System;
using System.Collections.Generic;

namespace ClinicOS.Application.Features.Receptionists.Commands.DeleteReceptionist;

[Permission(Permissions.Receptionists.Delete)]
public sealed record DeleteReceptionistCommand(Guid Id) : ICommand<bool>, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => [
        "receptionists-list",
        $"receptionist-details-{Id}"
    ];
}