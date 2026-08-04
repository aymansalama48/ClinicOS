using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Patients.Queries.GetPatients;

public sealed class GetPatientsQueryHandler
    : IQueryHandler<GetPatientsQuery, PagedResult<PatientResponse>>
{
    private readonly IReadDbContext _context;

    public GetPatientsQueryHandler(IReadDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<PatientResponse>>> Handle(
        GetPatientsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. »‰«¡ «·«” ⁄·«„ «·√”«”Ì
        var query = _context.Patients.AsQueryable();

        // 2.  ÿ»Ìﬁ «·»ÕÀ (·Ê „ÊŸ› «·«” ﬁ»«· ﬂ » Õ«Ã…)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim();
            query = query.Where(p =>
                p.FirstName.Contains(search) ||
                p.LastName.Contains(search) ||
                p.PhoneNumber.Contains(search));
        }

        // 3.  ›⁄Ì· AsNoTracking ··√œ«¡ «·⁄«·Ì
        var noTrackingQuery = _context.AsNoTracking(query);

        // 4. Õ”«» «·⁄œœ «·ﬂ·Ì („Â„ ··»«Ã‰Ì‘‰)
        var totalCount = await noTrackingQuery.CountAsync(cancellationToken);

        // 5.  ÿ»Ìﬁ «· — Ì» Ê ÕœÌœ «·’›Õ… Ê«·ÕﬁÊ·
        var paginatedQuery = noTrackingQuery
            .OrderByDescending(p => p.CreatedAt)
            .Skip(request.Parameters.Skip)
            .Take(request.Parameters.PageSize)
            .Select(p => new PatientResponse(
                p.Id,
                string.IsNullOrEmpty(p.MiddleName) ? p.FirstName + " " + p.LastName : p.FirstName + " " + p.MiddleName + " " + p.LastName,
                p.PhoneNumber,
                p.DateOfBirth,
                p.Gender,
                p.BloodType,
                p.ApplicationUserId != null //  —Ã„… ·„⁄·Ê„… IsAccountLinked
            ));

        // 6. «· ‰›Ì–
        var items = await paginatedQuery.ToListAsync(cancellationToken);

        // 7.  ÃÂÌ“ «·‰ ÌÃ…
        var metadata = new PaginationMetadata
        {
            CurrentPage = request.Parameters.PageNumber,
            PageSize = request.Parameters.PageSize,
            TotalCount = totalCount
        };

        return Result<PagedResult<PatientResponse>>.Success(new PagedResult<PatientResponse>
        {
            Items = items,
            Pagination = metadata
        });
    }
}
