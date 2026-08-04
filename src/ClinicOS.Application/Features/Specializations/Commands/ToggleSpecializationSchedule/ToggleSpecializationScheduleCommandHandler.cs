using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Errors.Specializations;
using ClinicOS.Domain.Common.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Specializations.Commands.ToggleSpecializationSchedule;

public sealed class ToggleSpecializationScheduleCommandHandler : ICommandHandler<ToggleSpecializationScheduleCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public ToggleSpecializationScheduleCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(ToggleSpecializationScheduleCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _context.Specializations
            .Include(s => s.Schedules)
            .FirstOrDefaultAsync(s => s.Id == request.SpecializationId, cancellationToken);

        if (specialization is null)
            return Result<bool>.Failure(SpecializationErrors.NotFound);

        specialization.ToggleScheduleStatus(request.ScheduleId);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
