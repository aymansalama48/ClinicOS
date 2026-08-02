using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Doctors; // استدعاء الأخطاء المنظمة
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctorAvailabilities;

public sealed class GetDoctorAvailabilitiesQueryHandler
    : IQueryHandler<GetDoctorAvailabilitiesQuery, PagedResult<DoctorAvailabilityResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetDoctorAvailabilitiesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<DoctorAvailabilityResponse>>> Handle(
        GetDoctorAvailabilitiesQuery request,
        CancellationToken cancellationToken)
    {
        // 1. التأكد من وجود الطبيب أولاً (باستخدام الـ Errors المنظمة)
        var doctorQuery = _context.Doctors.Where(d => d.Id == request.DoctorId);
        var doctorExists = await _context.AnyAsync(doctorQuery, cancellationToken);

        if (!doctorExists)
        {
            return Result<PagedResult<DoctorAvailabilityResponse>>.Failure(DoctorErrors.NotFound);
        }

        // 2. الاستعلام عن المواعيد
        var query = _context.DoctorAvailabilities
            .Where(da => da.DoctorId == request.DoctorId);
        // 👇 تطبيق فلتر اليوم لو الفرونت إند بعته
        if (request.DayOfWeek.HasValue)
        {
            query = query.Where(da => da.DayOfWeek == request.DayOfWeek.Value);
        }
        // 3. تفعيل AsNoTracking من الأداپتر (عشان الأداء)
        var noTrackingQuery = _context.AsNoTracking(query);

        // 4. حساب العدد الكلي (للـ Metadata بتاعت الباجنيشن)
        var totalCount = await _context.CountAsync(noTrackingQuery, cancellationToken);

        // 5. تطبيق الترتيب وتحديد الصفحة (Skip & Take)
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

        // 6. تنفيذ الاستعلام وجلب البيانات
        var items = await _context.ToListAsync(paginatedQuery, cancellationToken);

        // 7. بناء معلومات الباجنيشن (Metadata)
        var metadata = new PaginationMetadata
        {
            CurrentPage = request.Parameters.PageNumber,
            PageSize = request.Parameters.PageSize,
            TotalCount = totalCount
        };

        // 8. إرجاع النتيجة متغلفة في PagedResult
        return Result<PagedResult<DoctorAvailabilityResponse>>.Success(new PagedResult<DoctorAvailabilityResponse>
        {
            Items = items,
            Pagination = metadata
        });
    }
}