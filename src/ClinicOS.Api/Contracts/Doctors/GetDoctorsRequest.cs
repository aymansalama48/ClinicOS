using ClinicOS.Api.Contracts.Common;
using System;

namespace ClinicOS.Api.Contracts.Doctors;

public record GetDoctorsRequest : PaginationRequest
{
    public Guid? SpecializationId { get; init; }
    public bool? IsActive { get; init; }
}