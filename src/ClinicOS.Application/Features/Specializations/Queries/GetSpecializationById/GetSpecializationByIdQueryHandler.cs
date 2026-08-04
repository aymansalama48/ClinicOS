using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Specializations;
using ClinicOS.Application.Features.Specializations.Shared;
using ClinicOS.Domain.Common.Results;


namespace ClinicOS.Application.Features.Specializations.Queries.GetSpecializationById;

public sealed class GetSpecializationByIdQueryHandler : IQueryHandler<GetSpecializationByIdQuery, SpecializationDetailsResponse>
{
    private readonly IApplicationDbContext _context;

    public GetSpecializationByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SpecializationDetailsResponse>> Handle(GetSpecializationByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. جلب التخصص
        var query = _context.Specializations.Where(s => s.Id == request.Id && !s.IsDeleted);
        var noTrackingQuery = _context.AsNoTracking(query);
        var specialization = await _context.FirstOrDefaultAsync(noTrackingQuery, cancellationToken);

        if (specialization is null)
        {
            return Result<SpecializationDetailsResponse>.Failure(SpecializationErrors.NotFound);
        }

        // 2. جلب المواعيد الخاصة بالتخصص ده
        var schedulesQuery = _context.SpecializationSchedules.Where(ss => ss.SpecializationId == request.Id);
        var noTrackingSchedulesQuery = _context.AsNoTracking(schedulesQuery);
        var schedules = await _context.ToListAsync(noTrackingSchedulesQuery, cancellationToken);

        // 3. تحويل المواعيد للـ Response بالترتيب اللي إنت طالبه بالظبط
        var scheduleResponses = schedules.Select(s => new SpecializationScheduleResponse(
            s.Id,
            s.DayOfWeek,
            s.Period,
            s.StartTime,
            s.EndTime,
            s.IsActive
        )).ToList();

        // 4. بناء الرد النهائي وتمرير اللستة
        var response = new SpecializationDetailsResponse(
            specialization.Id,
            specialization.Name,
            specialization.Description,
            scheduleResponses
        );

        return Result<SpecializationDetailsResponse>.Success(response);
    }
}