using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;
using ClinicOS.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ClinicOS.Application.Features.Patients.Commands.CreatePatient;

[Permission(Permissions.Patients.Create)]
public sealed record CreatePatientCommand(
    string FirstName,
    string? MiddleName,
    string LastName,
    string PhoneNumber,
    DateOnly? DateOfBirth,
    Gender? Gender,
    BloodType? BloodType,
    string? EmergencyContact,
    Guid? ApplicationUserId
) : ICommand<Guid>, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => ["patients-list"];
}