using ClinicOS.Application.Common.Abstractions.Messaging;
using System.IO;
using System;

namespace ClinicOS.Application.Features.Attachments.Queries.DownloadAttachment;

public record AttachmentDownloadDto(Stream Content, string ContentType, string FileName);

public sealed record DownloadAttachmentQuery(Guid Id) : IQuery<AttachmentDownloadDto>;
