using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;
using System;
using System.Collections.Generic;

namespace ClinicOS.Application.Features.Specializations.Commands.CreateSpecialization;

[Permission(Permissions.Specializations.Create)]
public sealed record CreateSpecializationCommand(
    string Name,
    string? Description
) : ICommand<Guid>, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => ["specializations-list"];
}