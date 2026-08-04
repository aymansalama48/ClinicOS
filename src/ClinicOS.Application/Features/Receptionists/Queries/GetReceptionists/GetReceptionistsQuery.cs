using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Constants;
using ClinicOS.Domain.Common.Results;
using System;

namespace ClinicOS.Application.Features.Receptionists.Queries.GetReceptionists;

[Permission(Permissions.Receptionists.View)] //[cite: 49]
public sealed record GetReceptionistsQuery(
    Guid? SpecializationId,
    bool? IsActive,
    PaginationParameters Parameters
) : ICacheableQuery<PagedResult<ReceptionistResponse>> //[cite: 49]
{
    // 👇 التعديل هنا: استخدام Fallback لو القيم Null
    public string CacheKey => $"receptionists-list-spec-{SpecializationId?.ToString() ?? "all"}-active-{IsActive?.ToString() ?? "all"}-page-{Parameters.PageNumber}-size-{Parameters.PageSize}";

    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(10); //[cite: 49]
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromHours(1); //[cite: 49]
}