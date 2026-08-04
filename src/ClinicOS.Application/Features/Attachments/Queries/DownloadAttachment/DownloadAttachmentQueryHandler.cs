using ClinicOS.Application.Common.Abstractions.External.FileStorage;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Domain.Common.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Attachments.Queries.DownloadAttachment;

public sealed class DownloadAttachmentQueryHandler : IQueryHandler<DownloadAttachmentQuery, AttachmentDownloadDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorage _fileStorage;

    public DownloadAttachmentQueryHandler(IApplicationDbContext context, IFileStorage fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<Result<AttachmentDownloadDto>> Handle(DownloadAttachmentQuery request, CancellationToken cancellationToken)
    {
        var attachment = await _context.Attachments
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (attachment == null)
            return Result<AttachmentDownloadDto>.Failure(new Error("Attachment.NotFound", "المرفق غير موجود.", ErrorType.NotFound));

        var downloadResult = await _fileStorage.DownloadAsync(attachment.FileId, cancellationToken);

        if (downloadResult.IsFailure)
            return Result<AttachmentDownloadDto>.Failure(downloadResult.Errors);

        return Result<AttachmentDownloadDto>.Success(new AttachmentDownloadDto(
            downloadResult.Data,
            attachment.ContentType,
            attachment.FileName
        ));
    }
}
