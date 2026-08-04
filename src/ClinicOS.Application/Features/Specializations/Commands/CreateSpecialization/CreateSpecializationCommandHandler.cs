using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Errors.Specializations;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Specializations;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Application.Features.Specializations.Commands.CreateSpecialization;

public sealed class CreateSpecializationCommandHandler : ICommandHandler<CreateSpecializationCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateSpecializationCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<Guid>> Handle(CreateSpecializationCommand request, CancellationToken cancellationToken)
    {
        var isDuplicate = await _context.Specializations
            .AnyAsync(s => s.Name == request.Name, cancellationToken);

        if (isDuplicate)
            return Result<Guid>.Failure(SpecializationErrors.DuplicateName);

        // ÇÓÊÎÏÇã ÇáÜ Factory Method (DDD)
        var specialization = Specialization.Create(request.Name, request.Description, request.IconAttachmentId);

        if (request.IconAttachmentId.HasValue)
        {
            var attachment = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == request.IconAttachmentId.Value, cancellationToken);
            if (attachment != null && attachment.EntityId == null)
            {
                attachment.AssignToEntity(nameof(Specialization), specialization.Id);
            }
        }

        _context.Specializations.Add(specialization);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(specialization.Id);
    }
}