using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Doctors; // 👈 استدعاء كلاس الأخطاء
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Doctors;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Doctors.Commands.SetDoctorAvailability;

public sealed class SetDoctorAvailabilityCommandHandler
    : ICommandHandler<SetDoctorAvailabilityCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public SetDoctorAvailabilityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(
        SetDoctorAvailabilityCommand request,
        CancellationToken cancellationToken)
    {
        // 1. التحقق من وجود الطبيب
        var doctorQuery = _context.Doctors.Where(d => d.Id == request.DoctorId);
        var doctorExists = await _context.AnyAsync(doctorQuery, cancellationToken);

        if (!doctorExists)
        {
            // 👇 استخدام الـ Error المنظم
            return Result<Guid>.Failure(DoctorErrors.NotFound);
        }

        // 2. التحقق من عدم تكرار نفس الفترة
        var overlappingQuery = _context.DoctorAvailabilities.Where(da =>
            da.DoctorId == request.DoctorId &&
            da.DayOfWeek == request.DayOfWeek &&
            da.Period == request.Period);

        var isOverlapping = await _context.AnyAsync(overlappingQuery, cancellationToken);

        if (isOverlapping)
        {
            // 👇 استخدام الـ Error المنظم
            return Result<Guid>.Failure(DoctorErrors.AvailabilityConflict);
        }

        // 3. إنشاء الموعد
        var availability = DoctorAvailability.Create(
            request.DoctorId,
            request.DayOfWeek,
            request.Period,
            request.StartTime,
            request.EndTime,
            request.MaxPatients);

        // 4. الحفظ
        _context.Add(availability);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(availability.Id);
    }
}