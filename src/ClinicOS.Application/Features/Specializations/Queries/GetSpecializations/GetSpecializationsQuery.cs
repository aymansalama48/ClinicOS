using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Application.Features.Specializations.Shared;
using ClinicOS.Domain.Constants;
using ClinicOS.Domain.Common.Results;
using System;
using System.Text.Json.Serialization;

namespace ClinicOS.Application.Features.Specializations.Queries.GetSpecializations;

[Permission(Permissions.Specializations.View)] //[cite: 51]
public sealed record GetSpecializationsQuery(
    string? SearchTerm,
    PaginationParameters Parameters
) : ICacheableQuery<PagedResult<SpecializationResponse>> //[cite: 51]
{
    // 👇 معالجة الـ Null في كلمة البحث
    public string CacheKey => $"specializations-list-search-{SearchTerm ?? "all"}-page-{Parameters.PageNumber}-size-{Parameters.PageSize}";

    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(10); //[cite: 51]

    public TimeSpan? AbsoluteExpiration => TimeSpan.FromHours(1); //[cite: 51]
}