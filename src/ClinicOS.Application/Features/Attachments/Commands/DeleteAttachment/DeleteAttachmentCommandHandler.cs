using ClinicOS.Application.Common.Abstractions.External.FileStorage;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Domain.Common.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Attachments.Commands.DeleteAttachment;

public sealed class DeleteAttachmentCommandHandler : ICommandHandler<DeleteAttachmentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorage _fileStorage;

    public DeleteAttachmentCommandHandler(IApplicationDbContext context, IFileStorage fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<Result> Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
    {
        var attachment = await _context.Attachments
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (attachment == null)
            return Result.Failure(new Error("Attachment.NotFound", "المرفق غير موجود.", ErrorType.NotFound));

        // يتم الحذف فعلياً من درايف أولاً قبل الحذف من قاعدة البيانات
        var deleteResult = await _fileStorage.DeleteAsync(attachment.FileId, cancellationToken);
        if (deleteResult.IsFailure)
        {
            return deleteResult;
        }

        // الحذف من الداتا بيز (والـ EF سيتولى عملية الـ Soft Delete إذا كان مجهزاً لذلك)
        _context.Attachments.Remove(attachment);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
