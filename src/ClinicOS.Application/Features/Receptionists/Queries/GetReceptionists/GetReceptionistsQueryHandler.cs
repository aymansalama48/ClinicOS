using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Receptionists.Queries.GetReceptionists;

public sealed class GetReceptionistsQueryHandler
    : IQueryHandler<GetReceptionistsQuery, PagedResult<ReceptionistResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetReceptionistsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<ReceptionistResponse>>> Handle(
        GetReceptionistsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Receptionists.AsQueryable();

        // الفلترة
        if (request.SpecializationId.HasValue)
        {
            query = query.Where(r => r.SpecializationId == request.SpecializationId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(r => r.IsActive == request.IsActive.Value);
        }

        var noTrackingQuery = _context.AsNoTracking(query);
        var totalCount = await _context.CountAsync(noTrackingQuery, cancellationToken);

        var paginatedQuery = noTrackingQuery
            .OrderByDescending(r => r.CreatedAt)
            .Skip(request.Parameters.Skip)
            .Take(request.Parameters.PageSize)
            .Select(r => new ReceptionistResponse(
                r.Id,
                r.SpecializationId,
                r.ApplicationUserId,
                r.IsActive
            ));

        var items = await _context.ToListAsync(paginatedQuery, cancellationToken);

        var metadata = new PaginationMetadata
        {
            CurrentPage = request.Parameters.PageNumber,
            PageSize = request.Parameters.PageSize,
            TotalCount = totalCount
        };

        return Result<PagedResult<ReceptionistResponse>>.Success(new PagedResult<ReceptionistResponse>
        {
            Items = items,
            Pagination = metadata
        });
    }
}