using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Domain.Constants;
using ClinicOS.Domain.Common.Results;
using System;

namespace ClinicOS.Application.Features.Patients.Queries.GetPatientById;

[Permission(Permissions.Patients.View)] // 👈 الصلاحية الحقيقية من ملفك
public sealed record GetPatientByIdQuery(Guid Id) : ICacheableQuery<PatientDetailsResponse>
{
    public string CacheKey => $"patient-details-{Id}";
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(15);
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromHours(2);
}