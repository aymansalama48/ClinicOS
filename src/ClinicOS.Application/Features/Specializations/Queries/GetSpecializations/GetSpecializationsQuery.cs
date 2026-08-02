using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Application.Features.Specializations.Shared;
using ClinicOS.Domain.Constants;
using ClinicOS.Domain.Common.Results;
using System;
using System.Text.Json.Serialization;

namespace ClinicOS.Application.Features.Specializations.Queries.GetSpecializations;

[Permission(Permissions.Specializations.View)] // 👈 ضبط الصلاحية
public sealed record GetSpecializationsQuery(
    string? SearchTerm,
    PaginationParameters Parameters
) : ICacheableQuery<PagedResult<SpecializationResponse>>
{
    public string CacheKey => $"specializations-list-search-{SearchTerm}-page-{Parameters.PageNumber}-size-{Parameters.PageSize}";

    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(10);

    public TimeSpan? AbsoluteExpiration => TimeSpan.FromHours(1);
}