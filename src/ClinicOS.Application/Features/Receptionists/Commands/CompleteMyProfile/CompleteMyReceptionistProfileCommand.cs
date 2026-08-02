using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Messaging;
using System;
using System.Collections.Generic;

namespace ClinicOS.Application.Features.Receptionists.Commands.CompleteMyProfile;

public sealed record CompleteMyReceptionistProfileCommand(
    Guid SpecializationId
) : ICommand<Guid>, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => ["receptionists-list"];
}