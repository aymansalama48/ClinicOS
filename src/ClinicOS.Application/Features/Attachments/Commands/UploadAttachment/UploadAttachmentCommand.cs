using ClinicOS.Application.Common.Abstractions.Messaging;
using System.IO;
using System;

namespace ClinicOS.Application.Features.Attachments.Commands.UploadAttachment;

public sealed record UploadAttachmentCommand(
    Stream Content,
    string FileName,
    string ContentType,
    long Size,
    string EntityType,
    Guid? EntityId
) : ICommand<Guid>;