using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ClinicOS.Application.Features.Patients.Commands.UpdateMyProfile;

public sealed record UpdateMyPatientProfileCommand(
    string FirstName,
    string? MiddleName,
    string LastName,
    string PhoneNumber,
    DateOnly? DateOfBirth,
    Gender? Gender,
    BloodType? BloodType,
    string? EmergencyContact
) : ICommand<bool>, ICacheInvalidatorCommand
{
    public IReadOnlyCollection<string> CacheKeys => ["patients-list"];
}