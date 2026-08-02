using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Domain.Constants;
using System;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctorById;

[Permission(Permissions.Doctors.View)]
public sealed record GetDoctorByIdQuery(
    Guid Id
) : ICacheableQuery<DoctorDetailsResponse>
{
    // الكاش Key خاص بالدكتور ده بس
    public string CacheKey => $"doctor-details-{Id}";

    // ممكن نطول مدة الكاش هنا شوية لأن تفاصيل الدكتور مش بتتغير كل دقيقة
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(15);
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromHours(2);
}