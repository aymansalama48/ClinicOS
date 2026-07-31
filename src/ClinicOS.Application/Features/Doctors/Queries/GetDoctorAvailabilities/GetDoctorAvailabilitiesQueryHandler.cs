using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Common.Results;
using Microsoft.EntityFrameworkCore;
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
        // 1. تجهيز الاستعلام الأساسي (قراءة فقط بدون تتبع)
        var query = _context.DoctorAvailabilities
            .AsNoTracking()
            .Where(da => da.DoctorId == request.DoctorId);

        // 2. حساب إجمالي عدد العناصر (مهم جداً للـ Pagination Metadata)
        var totalCount = await query.CountAsync(cancellationToken);

        // 3. جلب البيانات المطلوبة للصفحة الحالية
        var items = await query
            .OrderBy(da => da.DayOfWeek) // ترتيب بالأيام
            .ThenBy(da => da.Period)     // ثم بالفترات
            .Skip(request.Parameters.Skip) // 👈 تخطي العناصر القديمة[cite: 10]
            .Take(request.Parameters.PageSize) // 👈 جلب حجم الصفحة المطلوب[cite: 10]
            .Select(da => new DoctorAvailabilityResponse(
                da.Id,
                da.DayOfWeek,
                da.Period,
                da.StartTime,
                da.EndTime,
                da.MaxPatients))
            .ToListAsync(cancellationToken);

        // 4. بناء معلومات الصفحة (Metadata)[cite: 9]
        var metadata = new PaginationMetadata
        {
            CurrentPage = request.Parameters.PageNumber,
            PageSize = request.Parameters.PageSize,
            TotalCount = totalCount
        };

        // 5. تجميع النتيجة النهائية[cite: 8]
        var pagedResult = new PagedResult<DoctorAvailabilityResponse>
        {
            Items = items,
            Pagination = metadata
        };

        // 6. إرجاع النتيجة بنجاح
        return Result<PagedResult<DoctorAvailabilityResponse>>.Success(pagedResult);
    }
}