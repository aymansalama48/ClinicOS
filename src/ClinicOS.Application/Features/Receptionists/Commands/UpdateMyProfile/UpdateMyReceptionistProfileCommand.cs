using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Messaging;
using System;
using System.Collections.Generic;

namespace ClinicOS.Application.Features.Receptionists.Commands.UpdateMyProfile;

public sealed record UpdateMyReceptionistProfileCommand(
    Guid SpecializationId
) : ICommand<Guid>, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => ["receptionists-list"];
}