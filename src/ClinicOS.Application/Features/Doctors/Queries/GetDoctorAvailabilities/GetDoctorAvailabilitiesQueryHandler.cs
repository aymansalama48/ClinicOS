using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Errors.Doctors; // «” œ⁄«¡ «·√Œÿ«¡ «·„‰Ÿ„…
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctorAvailabilities;

public sealed class GetDoctorAvailabilitiesQueryHandler
    : IQueryHandler<GetDoctorAvailabilitiesQuery, PagedResult<DoctorAvailabilityResponse>>
{
    private readonly IReadDbContext _context;

    public GetDoctorAvailabilitiesQueryHandler(IReadDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<DoctorAvailabilityResponse>>> Handle(
        GetDoctorAvailabilitiesQuery request,
        CancellationToken cancellationToken)
    {
        // 1. «· √ﬂœ „‰ ÊÃÊœ «·ÿ»Ì» √Ê·« (»«” Œœ«„ «·‹ Errors «·„‰Ÿ„…)
        var doctorQuery = _context.Doctors.Where(d => d.Id == request.DoctorId);
        var doctorExists = await doctorQuery.AnyAsync(cancellationToken);

        if (!doctorExists)
        {
            return Result<PagedResult<DoctorAvailabilityResponse>>.Failure(DoctorErrors.NotFound);
        }

        // 2. «·«” ⁄·«„ ⁄‰ «·„Ê«⁄Ìœ
        var query = _context.DoctorAvailabilities
            .Where(da => da.DoctorId == request.DoctorId);
        // ??  ÿ»Ìﬁ ›· — «·ÌÊ„ ·Ê «·›—Ê‰  ≈‰œ »⁄ Â
        if (request.DayOfWeek.HasValue)
        {
            query = query.Where(da => da.DayOfWeek == request.DayOfWeek.Value);
        }
        // 3.  ›⁄Ì· AsNoTracking „‰ «·√œ«Å — (⁄‘«‰ «·√œ«¡)
        var noTrackingQuery = _context.AsNoTracking(query);

        // 4. Õ”«» «·⁄œœ «·ﬂ·Ì (··‹ Metadata » «⁄  «·»«Ã‰Ì‘‰)
        var totalCount = await noTrackingQuery.CountAsync(cancellationToken);

        // 5.  ÿ»Ìﬁ «· — Ì» Ê ÕœÌœ «·’›Õ… (Skip & Take)
        var paginatedQuery = noTrackingQuery
            .OrderBy(da => da.DayOfWeek)
            .ThenBy(da => da.Period)
            .Skip(request.Parameters.Skip)
            .Take(request.Parameters.PageSize)
            .Select(da => new DoctorAvailabilityResponse(
                da.Id,
                da.DayOfWeek,
                da.Period,
                da.StartTime,
                da.EndTime,
                da.MaxPatients
            ));

        // 6.  ‰›Ì– «·«” ⁄·«„ ÊÃ·» «·»Ì«‰« 
        var items = await paginatedQuery.ToListAsync(cancellationToken);

        // 7. »‰«¡ „⁄·Ê„«  «·»«Ã‰Ì‘‰ (Metadata)
        var metadata = new PaginationMetadata
        {
            CurrentPage = request.Parameters.PageNumber,
            PageSize = request.Parameters.PageSize,
            TotalCount = totalCount
        };

        // 8. ≈—Ã«⁄ «·‰ ÌÃ… „ €·›… ›Ì PagedResult
        return Result<PagedResult<DoctorAvailabilityResponse>>.Success(new PagedResult<DoctorAvailabilityResponse>
        {
            Items = items,
            Pagination = metadata
        });
    }
}
