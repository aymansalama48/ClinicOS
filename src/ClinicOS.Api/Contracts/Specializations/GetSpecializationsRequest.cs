using ClinicOS.Api.Contracts.Common;

namespace ClinicOS.Api.Contracts.Specializations;

public record GetSpecializationsRequest : PaginationRequest
{
    public string? SearchTerm { get; init; }
}