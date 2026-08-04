using ClinicOS.Api.Contracts.Common;

namespace ClinicOS.Api.Contracts.Accounts;

public record GetUsersRequest : PaginationRequest
{
    public string? Role { get; init; }
    public string? SearchTerm { get; init; }
}