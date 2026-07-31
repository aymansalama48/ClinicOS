using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Doctors;
using System;
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
        // 1. التحقق من وجود الطبيب أولاً
        var doctorExists = await _context.Doctors
            .AnyAsync(d => d.Id == request.DoctorId, cancellationToken);

        if (!doctorExists)
        {
            return Result<Guid>.Failure(new Error(
                "Doctor.NotFound",
                "الطبيب المحدد غير موجود في النظام.",
                ErrorType.NotFound));
        }

        // 2. التحقق من عدم تكرار نفس الفترة في نفس اليوم للطبيب
        var isOverlapping = await _context.DoctorAvailabilities
            .AnyAsync(da =>
                da.DoctorId == request.DoctorId &&
                da.DayOfWeek == request.DayOfWeek &&
                da.Period == request.Period,
                cancellationToken);

        if (isOverlapping)
        {
            return Result<Guid>.Failure(new Error(
                "DoctorAvailability.Conflict",
                "يوجد موعد مسجل مسبقاً لهذا الطبيب في نفس اليوم والفترة المحددة.",
                ErrorType.Conflict));
        }

        // 3. إنشاء الموعد باستخدام الـ Factory Method اللي عملناها في الـ Domain
        var availability = DoctorAvailability.Create(
            request.DoctorId,
            request.DayOfWeek,
            request.Period,
            request.StartTime,
            request.EndTime,
            request.MaxPatients);

        // 4. الحفظ المباشر (باستخدام دوال الـ Adapter النظيفة)
        _context.Add(availability);
        await _context.SaveChangesAsync(cancellationToken);

        // 5. إرجاع النتيجة بنجاح مع الـ ID الجديد
        return Result<Guid>.Success(availability.Id);
    }
}