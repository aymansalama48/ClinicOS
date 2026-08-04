using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Errors.Specializations;
using ClinicOS.Domain.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Application.Features.Specializations.Commands.DeleteSpecialization;

public sealed class DeleteSpecializationCommandHandler : ICommandHandler<DeleteSpecializationCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteSpecializationCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(DeleteSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _context.Specializations
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (specialization is null)
            return Result<bool>.Failure(SpecializationErrors.NotFound);

        // سيقوم الـ SoftDeleteInterceptor بتحويل الحذف إلى حذف منطقي تلقائياً
        _context.Specializations.Remove(specialization);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}