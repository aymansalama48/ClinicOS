using ClinicOS.Application.Common.Abstractions.External.FileStorage;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Storage;
using ClinicOS.Application.Common.Abstractions.Attachments;
using System.IO;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Attachments.Commands.UploadAttachment;

public sealed class UploadAttachmentCommandHandler : ICommandHandler<UploadAttachmentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly IAttachmentEntityResolver _entityResolver;

    public UploadAttachmentCommandHandler(
        IApplicationDbContext context,
        IFileStorage fileStorage,
        IAttachmentEntityResolver entityResolver)
    {
        _context = context;
        _fileStorage = fileStorage;
        _entityResolver = entityResolver;
    }

    public async Task<Result<Guid>> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
    {
        if (!_entityResolver.TryResolve(request.EntityType, out var resolvedEntity))
        {
            return Result<Guid>.Failure(new Error("Attachment.InvalidEntity", "نوع الكيان غير مدعوم.", ErrorType.Validation));
        }

        var extension = Path.GetExtension(request.FileName);
        var storedName = $"{Guid.NewGuid():N}{extension}";
        var folderName = resolvedEntity.TableName;

        var uploadResult = await _fileStorage.UploadAsync(
            fileStream: request.Content,
            storedFileName: storedName,
            folderName: folderName,
            contentType: request.ContentType,
            cancellationToken: cancellationToken);

        if (uploadResult.IsFailure)
            return Result<Guid>.Failure(uploadResult.Errors);

        var storedFile = uploadResult.Data;

        var attachment = Attachment.Create(
            fileName: Path.GetFileName(request.FileName),
            storedName: storedName,
            contentType: request.ContentType,
            fileId: storedFile.FileId,
            sizeInBytes: request.Size, // الحجم المتحقق منه من الـ Command
            externalUrl: storedFile.WebViewLink,
            entityType: request.EntityType,
            entityId: request.EntityId
        );

        _context.Attachments.Add(attachment);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(attachment.Id);
        }
        catch (Exception)
        {
            // Compensation Pattern: حذف الملف بشكل آمن باستخدام CancellationToken.None حتى لا يتم إلغاءه
            await _fileStorage.DeleteAsync(storedFile.FileId, CancellationToken.None);
            throw;
        }
    }
}