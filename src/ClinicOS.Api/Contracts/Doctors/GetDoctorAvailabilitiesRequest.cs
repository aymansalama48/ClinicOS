using ClinicOS.Api.Contracts.Common;
using System;

namespace ClinicOS.Api.Contracts.Doctors;

public record GetDoctorAvailabilitiesRequest : PaginationRequest
{
    // فلترة اختيارية بيوم محدد في الأسبوع
    public DayOfWeek? DayOfWeek { get; init; }
}