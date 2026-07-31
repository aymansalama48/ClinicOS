using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Common.Results;
using System;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctorAvailabilities;

public sealed record GetDoctorAvailabilitiesQuery(
    Guid DoctorId,
    PaginationParameters Parameters // 👈 دمجنا الـ Pagination هنا
) : ICacheableQuery<PagedResult<DoctorAvailabilityResponse>> // 👈 نوع الإرجاع PagedResult
{
    // 💡 الكاش Key لازم يتغير مع كل صفحة عشان الداتا متتدخلش في بعضها
    public string CacheKey => $"doctor-availability-{DoctorId}-page-{Parameters.PageNumber}-size-{Parameters.PageSize}";

    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(10);
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromHours(1);
}