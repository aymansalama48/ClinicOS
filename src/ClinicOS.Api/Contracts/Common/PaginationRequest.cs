namespace ClinicOS.Api.Contracts.Common;

public record PaginationRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}