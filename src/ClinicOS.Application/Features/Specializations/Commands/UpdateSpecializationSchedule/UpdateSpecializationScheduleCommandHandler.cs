using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Errors.Specializations;
using ClinicOS.Domain.Common.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Specializations.Commands.UpdateSpecializationSchedule;

public sealed class UpdateSpecializationScheduleCommandHandler : ICommandHandler<UpdateSpecializationScheduleCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateSpecializationScheduleCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(UpdateSpecializationScheduleCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _context.Specializations
            .Include(s => s.Schedules)
            .FirstOrDefaultAsync(s => s.Id == request.SpecializationId, cancellationToken);

        if (specialization is null)
            return Result<bool>.Failure(SpecializationErrors.NotFound);

        specialization.UpdateSchedule(request.ScheduleId, request.DayOfWeek, request.Period, request.StartTime, request.EndTime);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
