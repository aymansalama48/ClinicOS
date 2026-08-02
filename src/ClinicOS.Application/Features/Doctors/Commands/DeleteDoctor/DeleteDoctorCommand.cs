using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;
using System;
using System.Collections.Generic;

namespace ClinicOS.Application.Features.Doctors.Commands.DeleteDoctor;

[Permission(Permissions.Doctors.Delete)]
public sealed record DeleteDoctorCommand(Guid Id) : ICommand<bool>, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => [
        "doctors-list",
        $"doctor-details-{Id}"
    ];
}