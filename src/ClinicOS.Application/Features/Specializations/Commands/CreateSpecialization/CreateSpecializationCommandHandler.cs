using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Specializations;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Specializations; // 👈 الكيان بتاعك
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ClinicOS.Application.Features.Specializations.Commands.CreateSpecialization;

public sealed class CreateSpecializationCommandHandler : ICommandHandler<CreateSpecializationCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateSpecializationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateSpecializationCommand request, CancellationToken cancellationToken)
    {
        // التحقق من عدم تكرار الاسم
        var isDuplicate = await _context.AnyAsync(
            _context.Specializations.Where(s => s.Name == request.Name),
            cancellationToken);

        if (isDuplicate)
        {
            return Result<Guid>.Failure(SpecializationErrors.DuplicateName);
        }

        var specialization = new Specialization(request.Name, request.Description);

        _context.Add(specialization);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(specialization.Id);
    }
}