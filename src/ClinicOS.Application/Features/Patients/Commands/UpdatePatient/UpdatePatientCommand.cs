using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Constants;
using ClinicOS.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ClinicOS.Application.Features.Patients.Commands.UpdatePatient;

[Permission(Permissions.Patients.Update)] // 👈 الصلاحية الصحيحة
public sealed record UpdatePatientCommand(
    Guid PatientId,
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
    // 👇 هيمسح كاش القوائم كلها + كاش تفاصيل المريض ده تحديداً
    public IReadOnlyCollection<string> CacheKeys => [
        "patients-list",
        $"patient-details-{PatientId}"
    ];
}