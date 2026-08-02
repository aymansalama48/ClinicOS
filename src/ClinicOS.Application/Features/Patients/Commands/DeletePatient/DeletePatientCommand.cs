using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;
using System;
using System.Collections.Generic;

namespace ClinicOS.Application.Features.Patients.Commands.DeletePatient;

[Permission(Permissions.Patients.Delete)]
public sealed record DeletePatientCommand(Guid Id) : ICommand<bool>, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => [
        "patients-list",
        $"patient-details-{Id}"
    ];
}