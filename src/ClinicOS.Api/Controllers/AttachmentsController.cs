using System;
using System.Threading;
using System.Threading.Tasks;
using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Features.Attachments.Commands.UploadAttachment;
using ClinicOS.Application.Features.Attachments.Commands.DeleteAttachment;
using ClinicOS.Application.Features.Attachments.Queries.DownloadAttachment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOS.Api.Controllers;

[Authorize]
public class AttachmentsController : BaseApiController
{
    [HttpPost("upload")]
    [RequestSizeLimit(10 * 1024 * 1024)] // تطبيق حد أقصى للرفع (حوالي 10MB) ويتم التحقق الدقيق في الـ Validator بناءً على الإعدادات
    public async Task<IResult> Upload(
        [FromForm] IFormFile file, 
        [FromForm] string entityType, 
        [FromForm] Guid? entityId, 
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return Results.BadRequest("يجب إرفاق ملف صالح.");
        }

        using var stream = file.OpenReadStream();

        var command = new UploadAttachmentCommand(
            Content: stream,
            FileName: file.FileName,
            ContentType: file.ContentType,
            Size: file.Length,
            EntityType: entityType,
            EntityId: entityId
        );

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id}/download")]
    public async Task<IResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DownloadAttachmentQuery(id), cancellationToken);

        if (result.IsFailure)
            return HandleResult(result);

        var fileDto = result.Data!;
        // إرجاع FileStreamResult باستخدام Results.File
        return Results.File(fileDto.Content, fileDto.ContentType, fileDto.FileName);
    }

    [HttpDelete("{id}")]
    public async Task<IResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeleteAttachmentCommand(id), cancellationToken);
        return HandleResult(result);
    }
}