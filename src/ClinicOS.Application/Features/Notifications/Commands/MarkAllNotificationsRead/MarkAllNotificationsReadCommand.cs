namespace ClinicOS.Application.Features.Notifications.Commands.MarkAllNotificationsRead;

using ClinicOS.Domain.Common.Results;
using MediatR;

public record MarkAllNotificationsReadCommand() : IRequest<Result>;
