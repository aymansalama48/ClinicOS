namespace ClinicOS.Application.Features.Notifications.Commands.MarkNotificationRead;

using ClinicOS.Domain.Common.Results;
using MediatR;

public record MarkNotificationReadCommand(Guid NotificationId) : IRequest<Result>;
