using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Application.Features.Specializations.Shared;
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Specializations.Queries.GetSpecializations;

public sealed class GetSpecializationsQueryHandler : IQueryHandler<GetSpecializationsQuery, PagedResult<SpecializationResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetSpecializationsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<PagedResult<SpecializationResponse>>> Handle(GetSpecializationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Specializations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(s => s.Name.Contains(request.SearchTerm));
        }

        var noTrackingQuery = _context.AsNoTracking(query);
        var totalCount = await _context.CountAsync(noTrackingQuery, cancellationToken);

        var items = await _context.ToListAsync(
            noTrackingQuery
                .OrderBy(s => s.Name)
                .Skip(request.Parameters.Skip)
                .Take(request.Parameters.PageSize)
                .Select(s => new SpecializationResponse(s.Id, s.Name, s.Description)),
            cancellationToken);

        return Result<PagedResult<SpecializationResponse>>.Success(new PagedResult<SpecializationResponse>
        {
            Items = items,
            Pagination = new PaginationMetadata
            {
                CurrentPage = request.Parameters.PageNumber,
                PageSize = request.Parameters.PageSize,
                TotalCount = totalCount
            }
        });
    }
}