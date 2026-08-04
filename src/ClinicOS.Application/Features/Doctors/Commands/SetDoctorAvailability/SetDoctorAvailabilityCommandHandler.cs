using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Errors.Doctors; // ?? «” œ⁄«¡ ﬂ·«” «·√Œÿ«¡
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
        // 1. «· Õﬁﬁ „‰ ÊÃÊœ «·ÿ»Ì»
        var doctorQuery = _context.Doctors.Where(d => d.Id == request.DoctorId);
        var doctorExists = await doctorQuery.AnyAsync(cancellationToken);

        if (!doctorExists)
        {
            // ?? «” Œœ«„ «·‹ Error «·„‰Ÿ„
            return Result<Guid>.Failure(DoctorErrors.NotFound);
        }

        // 2. «· Õﬁﬁ „‰ ⁄œ„  ﬂ—«— ‰›” «·› —…
        var overlappingQuery = _context.DoctorAvailabilities.Where(da =>
            da.DoctorId == request.DoctorId &&
            da.DayOfWeek == request.DayOfWeek &&
            da.Period == request.Period);

        var isOverlapping = await overlappingQuery.AnyAsync(cancellationToken);

        if (isOverlapping)
        {
            // ?? «” Œœ«„ «·‹ Error «·„‰Ÿ„
            return Result<Guid>.Failure(DoctorErrors.AvailabilityConflict);
        }

        // 3. ≈‰‘«¡ «·„Ê⁄œ
        var availability = DoctorAvailability.Create(
            request.DoctorId,
            request.DayOfWeek,
            request.Period,
            request.StartTime,
            request.EndTime,
            request.MaxPatients);

        // 4. «·Õ›Ÿ
        _context.DoctorAvailabilities.Add(availability);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(availability.Id);
    }
}
