using ClinicOS.Api.Contracts.Common;

namespace ClinicOS.Api.Contracts.Patients;

public record GetPatientsRequest : PaginationRequest
{
    public string? SearchTerm { get; init; }
}