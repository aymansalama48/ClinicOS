using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Errors.Specializations;
using ClinicOS.Domain.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Application.Features.Specializations.Commands.RemoveSpecializationSchedule;

public sealed class RemoveSpecializationScheduleCommandHandler : ICommandHandler<RemoveSpecializationScheduleCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public RemoveSpecializationScheduleCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(RemoveSpecializationScheduleCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _context.Specializations
            .Include(s => s.Schedules)
            .FirstOrDefaultAsync(s => s.Id == request.SpecializationId, cancellationToken);

        if (specialization is null)
            return Result<bool>.Failure(SpecializationErrors.NotFound);

        // دالة الكيان ستحذفه من القائمة، وسيقوم EF Core و SoftDeleteInterceptor بالباقي!
        specialization.RemoveSchedule(request.ScheduleId);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}