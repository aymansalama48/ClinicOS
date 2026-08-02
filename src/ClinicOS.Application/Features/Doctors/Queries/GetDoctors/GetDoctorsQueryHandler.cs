using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctors;

public sealed class GetDoctorsQueryHandler
    : IQueryHandler<GetDoctorsQuery, PagedResult<DoctorResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserManagementService _userService;

    public GetDoctorsQueryHandler(
        IApplicationDbContext context,
        IUserManagementService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<Result<PagedResult<DoctorResponse>>> Handle(
        GetDoctorsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. بناء الاستعلام الأساسي
        var query = _context.Doctors.AsQueryable();

        // 2. تطبيق الفلاتر
        if (request.SpecializationId.HasValue)
        {
            query = query.Where(d => d.SpecializationId == request.SpecializationId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(d => d.IsActive == request.IsActive.Value);
        }

        var noTrackingQuery = _context.AsNoTracking(query);
        var totalCount = await _context.CountAsync(noTrackingQuery, cancellationToken);

        // 3. جلب الأطباء من قاعدة البيانات بدون ربط اليوزرز
        var paginatedDoctors = await _context.ToListAsync(
            noTrackingQuery
                .OrderByDescending(d => d.CreatedAt)
                .Skip(request.Parameters.Skip)
                .Take(request.Parameters.PageSize),
            cancellationToken);

        // 4. استخراج IDs المستخدمين لجلب بياناتهم
        var userIds = paginatedDoctors.Select(d => d.ApplicationUserId).Distinct().ToList();

        // 5. جلب بيانات المستخدمين من خدمة Identity وتخزينها في Dictionary لسرعة البحث
        var users = await _userService.GetUsersByIdsAsync(userIds, cancellationToken);
        var usersDict = users.ToDictionary(u => u.Id);

        // 6. الربط في الميموري وتجهيز الـ DTO النهائي
        var items = paginatedDoctors.Select(d =>
        {
            var user = usersDict.GetValueOrDefault(d.ApplicationUserId);
            return new DoctorResponse(
                d.Id,
                d.SpecializationId,
                d.ApplicationUserId,
                user?.FullName ?? "مستخدم غير معروف", // الاسم
                user?.Email,                          // الإيميل
                d.Bio,
                d.YearsOfExperience,
                d.ConsultationFee,
                d.UrgentSurchargeFee,
                d.IsActive
            );
        }).ToList();

        // 7. تجهيز النتيجة
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