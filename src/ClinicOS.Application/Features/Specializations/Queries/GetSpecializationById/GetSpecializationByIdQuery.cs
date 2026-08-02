using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Features.Specializations.Shared;
using ClinicOS.Domain.Constants;
using System;

namespace ClinicOS.Application.Features.Specializations.Queries.GetSpecializationById;

[Permission(Permissions.Specializations.View)]
public sealed record GetSpecializationByIdQuery(Guid Id) : ICacheableQuery<SpecializationDetailsResponse>
{
    public string CacheKey => $"specialization-details-{Id}";
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(15);
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromHours(2);
}