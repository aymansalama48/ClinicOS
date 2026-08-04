using System;
using System.Collections.Generic;
using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Constants;
using ClinicOS.Domain.Common.Results;
using System;

namespace ClinicOS.Application.Features.Patients.Queries.GetPatients;

[Permission(Permissions.Patients.View)]
public sealed record GetPatientsQuery(
    string? SearchTerm, // 👈 للبحث السريع بالاسم أو التليفون
    PaginationParameters Parameters
) : ICacheableQuery<PagedResult<PatientResponse>>
{
    // الكاش Key بيتغير بناءً على كلمة البحث ورقم الصفحة
    public string CacheKey => $"patients-list-search-{SearchTerm ?? "all"}-page-{Parameters.PageNumber}-size-{Parameters.PageSize}";
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(10);
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromHours(1);
}