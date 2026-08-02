using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Specializations;
using ClinicOS.Domain.Common.Results;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ClinicOS.Application.Features.Specializations.Commands.UpdateSpecialization;

public sealed class UpdateSpecializationCommandHandler : ICommandHandler<UpdateSpecializationCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public UpdateSpecializationCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<Guid>> Handle(UpdateSpecializationCommand request, CancellationToken cancellationToken)
    {
        var isDuplicate = await _context.AnyAsync(
            _context.Specializations.Where(s => s.Name == request.Name && s.Id != request.SpecializationId),
            cancellationToken);

        if (isDuplicate)
        {
            return Result<Guid>.Failure(SpecializationErrors.DuplicateName);
        }

        var specialization = await _context.FirstOrDefaultAsync(
            _context.Specializations.Where(s => s.Id == request.SpecializationId),
            cancellationToken);

        if (specialization is null)
        {
            return Result<Guid>.Failure(SpecializationErrors.NotFound);
        }

        specialization.Name = request.Name;
        specialization.Description = request.Description;

        _context.Update(specialization);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(specialization.Id);
    }
}