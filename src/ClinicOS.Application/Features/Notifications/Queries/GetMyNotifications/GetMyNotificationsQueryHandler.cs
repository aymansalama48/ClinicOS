namespace ClinicOS.Application.Features.Notifications.Queries.GetMyNotifications;

using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Notifications;
using MediatR;

public class GetMyNotificationsQueryHandler(
    IReadDbContext dbContext,
    ICurrentUser currentUser)
    : IRequestHandler<GetMyNotificationsQuery, Result<PagedResult<NotificationDto>>>
{
    public async Task<Result<PagedResult<NotificationDto>>> Handle(
        GetMyNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Notifications
            .Where(n => n.UserId == currentUser.UserId.Value);

        if (request.UnreadOnly == true)
        {
            query = query.Where(n => !n.IsRead);
        }

        var totalCount = await dbContext.CountAsync(query, cancellationToken);

        var itemsQuery = query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new NotificationDto(
                n.Id,
                n.Title,
                n.Message,
                n.Type,
                n.IsRead,
                n.ActionUrl,
                n.CreatedAt));
                
        var items = await dbContext.ToListAsync(itemsQuery, cancellationToken);

        var pagedResult = new PagedResult<NotificationDto>
        {
            Items = items,
            Pagination = new PaginationMetadata
            {
                CurrentPage = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            }
        };

        return Result<PagedResult<NotificationDto>>.Success(pagedResult);
    }
}
