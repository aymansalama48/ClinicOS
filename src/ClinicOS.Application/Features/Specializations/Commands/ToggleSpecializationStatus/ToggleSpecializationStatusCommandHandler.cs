using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Errors.Specializations;
using ClinicOS.Domain.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Application.Features.Specializations.Commands.ToggleSpecializationStatus;

public sealed class ToggleSpecializationStatusCommandHandler : ICommandHandler<ToggleSpecializationStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public ToggleSpecializationStatusCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(ToggleSpecializationStatusCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _context.Specializations
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (specialization is null)
            return Result<bool>.Failure(SpecializationErrors.NotFound);

        // التبديل بين الحالتين واستدعاء الدوال التي ستُطلق الـ Events
        if (specialization.IsActive)
            specialization.Deactivate();
        else
            specialization.Activate();

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(specialization.IsActive);
    }
}