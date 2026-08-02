using ClinicOS.Api.Contracts.Common;
using System;

namespace ClinicOS.Api.Contracts.Receptionists;

public record GetReceptionistsRequest : PaginationRequest
{
    public Guid? SpecializationId { get; init; }
    public bool? IsActive { get; init; }
}