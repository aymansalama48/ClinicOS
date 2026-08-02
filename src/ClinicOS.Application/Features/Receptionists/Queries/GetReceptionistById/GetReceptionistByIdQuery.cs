using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Domain.Constants;
using ClinicOS.Domain.Common.Results;
using System;

namespace ClinicOS.Application.Features.Receptionists.Queries.GetReceptionistById;

[Permission(Permissions.Receptionists.View)]
public sealed record GetReceptionistByIdQuery(Guid Id) : ICacheableQuery<ReceptionistDetailsResponse>
{
    public string CacheKey => $"receptionist-details-{Id}";
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(15);
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromHours(2);
}