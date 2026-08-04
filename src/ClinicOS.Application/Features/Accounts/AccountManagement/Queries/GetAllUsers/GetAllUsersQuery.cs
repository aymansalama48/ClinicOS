using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Pagination;
using System;

namespace ClinicOS.Application.Features.Accounts.AccountManagement.Queries.GetAllUsers;

public sealed record GetAllUsersQuery(
    int PageNumber,
    int PageSize,
    string? Role,
    string? SearchTerm
) : ICacheableQuery<PagedResult<UserDto>>
{
    public string CacheKey => $"users-list-{PageNumber}-{PageSize}-{Role ?? "all"}-{SearchTerm ?? "none"}";
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(2);
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromMinutes(10);
}