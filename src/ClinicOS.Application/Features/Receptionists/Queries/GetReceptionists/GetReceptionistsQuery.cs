using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Constants;
using ClinicOS.Domain.Common.Results;
using System;

namespace ClinicOS.Application.Features.Receptionists.Queries.GetReceptionists;

[Permission(Permissions.Receptionists.View)]
public sealed record GetReceptionistsQuery(
    Guid? SpecializationId, // فلترة بالتخصص
    bool? IsActive,         // فلترة بالنشاط
    PaginationParameters Parameters
) : ICacheableQuery<PagedResult<ReceptionistResponse>>
{
    public string CacheKey => $"receptionists-list-spec-{SpecializationId}-active-{IsActive}-page-{Parameters.PageNumber}-size-{Parameters.PageSize}";

    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(10);
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromHours(1);
}
