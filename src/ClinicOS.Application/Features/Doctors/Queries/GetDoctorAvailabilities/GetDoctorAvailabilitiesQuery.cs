using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Constants;
using System;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctorAvailabilities;

[Permission(Permissions.Doctors.View)]
public sealed record GetDoctorAvailabilitiesQuery(
    Guid DoctorId,
    DayOfWeek? DayOfWeek,
    PaginationParameters Parameters
) : ICacheableQuery<PagedResult<DoctorAvailabilityResponse>>
{
    // 👈 ضفنا اليوم (DayOfWeek) في المفتاح، ولو المريض مابحثش بيوم معين هنكتب "all"
    public string CacheKey => $"doctor-availability-{DoctorId}-day-{DayOfWeek?.ToString() ?? "all"}-page-{Parameters.PageNumber}-size-{Parameters.PageSize}";

    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(10);
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromHours(1);
}