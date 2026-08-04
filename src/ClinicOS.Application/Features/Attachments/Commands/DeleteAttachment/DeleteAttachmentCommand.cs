using ClinicOS.Application.Common.Abstractions.Messaging;
using System;

namespace ClinicOS.Application.Features.Attachments.Commands.DeleteAttachment;

public sealed record DeleteAttachmentCommand(Guid Id) : ICommand;
