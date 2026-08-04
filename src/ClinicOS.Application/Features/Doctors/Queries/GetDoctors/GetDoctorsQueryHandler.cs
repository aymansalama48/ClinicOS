using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctors;

public sealed class GetDoctorsQueryHandler
    : IQueryHandler<GetDoctorsQuery, PagedResult<DoctorResponse>>
{
    private readonly IReadDbContext _context;
    private readonly IUserManagementService _userService;

    public GetDoctorsQueryHandler(
        IReadDbContext context,
        IUserManagementService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<Result<PagedResult<DoctorResponse>>> Handle(
        GetDoctorsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. »‰«¡ «·«” ⁄·«„ «·√”«”Ì
        var query = _context.Doctors.AsQueryable();

        // 2.  ÿ»Ìﬁ «·›·« —
        if (request.SpecializationId.HasValue)
        {
            query = query.Where(d => d.SpecializationId == request.SpecializationId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(d => d.IsActive == request.IsActive.Value);
        }

        var noTrackingQuery = _context.AsNoTracking(query);
        var totalCount = await noTrackingQuery.CountAsync(cancellationToken);

        // 3. Ã·» «·√ÿ»«¡ „‰ ﬁ«⁄œ… «·»Ì«‰«  »œÊ‰ —»ÿ «·ÌÊ“—“
        var paginatedDoctors = await 
            noTrackingQuery
                .OrderByDescending(d => d.CreatedAt)
                .Skip(request.Parameters.Skip)
                .Take(request.Parameters.PageSize).ToListAsync(cancellationToken);

        // 4. «” Œ—«Ã IDs «·„” Œœ„Ì‰ ·Ã·» »Ì«‰« Â„
        var userIds = paginatedDoctors.Select(d => d.ApplicationUserId).Distinct().ToList();

        // 5. Ã·» »Ì«‰«  «·„” Œœ„Ì‰ „‰ Œœ„… Identity Ê Œ“Ì‰Â« ›Ì Dictionary ·”—⁄… «·»ÕÀ
        var users = await _userService.GetUsersByIdsAsync(userIds, cancellationToken);
        var usersDict = users.ToDictionary(u => u.Id);

        // 6. «·—»ÿ ›Ì «·„Ì„Ê—Ì Ê ÃÂÌ“ «·‹ DTO «·‰Â«∆Ì
        var items = paginatedDoctors.Select(d =>
        {
            var user = usersDict.GetValueOrDefault(d.ApplicationUserId);
            return new DoctorResponse(
                d.Id,
                d.SpecializationId,
                d.ApplicationUserId,
                user?.FullName ?? "„” Œœ„ €Ì— „⁄—Ê›", // «·«”„
                user?.Email,                          // «·≈Ì„Ì·
                d.Bio,
                d.YearsOfExperience,
                d.ConsultationFee,
                d.UrgentSurchargeFee,
                d.IsActive
            );
        }).ToList();

        // 7.  ÃÂÌ“ «·‰ ÌÃ…
        var metadata = new PaginationMetadata
        {
            CurrentPage = request.Parameters.PageNumber,
            PageSize = request.Parameters.PageSize,
            TotalCount = totalCount
        };

        return Result<PagedResult<DoctorResponse>>.Success(new PagedResult<DoctorResponse>
        {
            Items = items,
            Pagination = metadata
        });
    }
}
