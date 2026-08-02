using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Constants;
using System;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctorAvailabilities;

[Permission(Permissions.Doctors.View)]
public sealed record GetDoctorAvailabilitiesQuery(
    Guid DoctorId,
    DayOfWeek? DayOfWeek, // 👈 ضفنا الفلتر هنا
    PaginationParameters Parameters
) : ICacheableQuery<PagedResult<DoctorAvailabilityResponse>>
{
    // الكاش Key بيتغير مع كل صفحة عشان الداتا متضربش في بعض
    public string CacheKey => $"doctor-availability-{DoctorId}-page-{Parameters.PageNumber}-size-{Parameters.PageSize}";
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(10);
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromHours(1);
}