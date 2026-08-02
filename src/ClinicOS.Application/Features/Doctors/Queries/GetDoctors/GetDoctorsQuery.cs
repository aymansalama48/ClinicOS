using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Constants;
using ClinicOS.Domain.Common.Results;
using System;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctors;

[Permission(Permissions.Doctors.View)]
public sealed record GetDoctorsQuery(
    Guid? SpecializationId,
    bool? IsActive,
    PaginationParameters Parameters
) : ICacheableQuery<PagedResult<DoctorResponse>>
{
    // 👇 الكاش Key بيتغير حسب الفلترة ورقم الصفحة
    public string CacheKey => $"doctors-list-spec-{SpecializationId}-active-{IsActive}-page-{Parameters.PageNumber}-size-{Parameters.PageSize}";

    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(10);
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromHours(1);
}