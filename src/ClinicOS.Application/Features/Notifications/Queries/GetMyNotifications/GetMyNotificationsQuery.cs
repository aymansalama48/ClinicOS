namespace ClinicOS.Application.Features.Notifications.Queries.GetMyNotifications;

using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Common.Results;
using MediatR;

public record GetMyNotificationsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    bool? UnreadOnly = null) : IRequest<Result<PagedResult<NotificationDto>>>;
