using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Patients.Queries.GetPatients;

public sealed class GetPatientsQueryHandler
    : IQueryHandler<GetPatientsQuery, PagedResult<PatientResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetPatientsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<PatientResponse>>> Handle(
        GetPatientsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. بناء الاستعلام الأساسي
        var query = _context.Patients.AsQueryable();

        // 2. تطبيق البحث (لو موظف الاستقبال كتب حاجة)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim();
            query = query.Where(p =>
                p.FirstName.Contains(search) ||
                p.LastName.Contains(search) ||
                p.PhoneNumber.Contains(search));
        }

        // 3. تفعيل AsNoTracking للأداء العالي
        var noTrackingQuery = _context.AsNoTracking(query);

        // 4. حساب العدد الكلي (مهم للباجنيشن)
        var totalCount = await _context.CountAsync(noTrackingQuery, cancellationToken);

        // 5. تطبيق الترتيب وتحديد الصفحة والحقول
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
                p.ApplicationUserId != null // ترجمة لمعلومة IsAccountLinked
            ));

        // 6. التنفيذ
        var items = await _context.ToListAsync(paginatedQuery, cancellationToken);

        // 7. تجهيز النتيجة
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