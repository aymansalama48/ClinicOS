using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Errors.Specializations;
using ClinicOS.Domain.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Application.Features.Specializations.Commands.AddSpecializationSchedule;

public sealed class AddSpecializationScheduleCommandHandler : ICommandHandler<AddSpecializationScheduleCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AddSpecializationScheduleCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<Guid>> Handle(AddSpecializationScheduleCommand request, CancellationToken cancellationToken)
    {
        // يجب عمل Include للـ Schedules لكي يتمكن Entity Framework من تتبع الإضافة الجديدة
        var specialization = await _context.Specializations
            .Include(s => s.Schedules)
            .FirstOrDefaultAsync(s => s.Id == request.SpecializationId, cancellationToken);

        if (specialization is null)
            return Result<Guid>.Failure(SpecializationErrors.NotFound);

        // التأكد من عدم تداخل المواعيد (Business Rule)
        var hasConflict = specialization.Schedules.Any(s =>
            s.DayOfWeek == request.DayOfWeek &&
            s.Period == request.Period &&
            s.IsDeleted == false);

        if (hasConflict)
            return Result<Guid>.Failure(SpecializationErrors.ScheduleConflict); // ضف هذا الـ Error في ملف الأخطاء عندك

        specialization.AddSchedule(
            request.DayOfWeek,
            request.Period,
            request.StartTime,
            request.EndTime);

        var newSchedule = specialization.Schedules.Last();
        _context.SpecializationSchedules.Add(newSchedule);

        await _context.SaveChangesAsync(cancellationToken);

        // إرجاع ID الموعد الجديد (الذي تم إضافته أخيرًا للقائمة)
        return Result<Guid>.Success(newSchedule.Id);
    }
}