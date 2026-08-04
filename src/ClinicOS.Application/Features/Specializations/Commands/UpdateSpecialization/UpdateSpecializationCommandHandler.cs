using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Errors.Specializations;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Specializations;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Application.Features.Specializations.Commands.UpdateSpecialization;

public sealed class UpdateSpecializationCommandHandler : ICommandHandler<UpdateSpecializationCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateSpecializationCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(UpdateSpecializationCommand request, CancellationToken cancellationToken)
    {
        var specialization = await _context.Specializations
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (specialization is null)
            return Result<bool>.Failure(SpecializationErrors.NotFound);

        // التأكد أن الاسم الجديد غير مكرر مع تخصص آخر
        var isDuplicate = await _context.Specializations
            .AnyAsync(s => s.Name == request.Name && s.Id != request.Id, cancellationToken);

        if (isDuplicate)
            return Result<bool>.Failure(SpecializationErrors.DuplicateName);

        // استخدام الـ Behavior Method لتعديل البيانات بدلاً من الـ Setters المباشرة (DDD)
        specialization.UpdateDetails(request.Name, request.Description, request.IconAttachmentId);

        if (request.IconAttachmentId.HasValue)
        {
            var attachment = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == request.IconAttachmentId.Value, cancellationToken);
            if (attachment != null && attachment.EntityId == null)
            {
                attachment.AssignToEntity(nameof(Specialization), specialization.Id);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}