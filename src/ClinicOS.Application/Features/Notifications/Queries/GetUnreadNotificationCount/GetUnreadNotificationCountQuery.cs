namespace ClinicOS.Application.Features.Notifications.Queries.GetUnreadNotificationCount;

using ClinicOS.Domain.Common.Results;
using MediatR;

public record GetUnreadNotificationCountQuery() : IRequest<Result<int>>;
