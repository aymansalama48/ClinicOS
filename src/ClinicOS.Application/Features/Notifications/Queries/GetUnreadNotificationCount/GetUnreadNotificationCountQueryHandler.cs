namespace ClinicOS.Application.Features.Notifications.Queries.GetUnreadNotificationCount;

using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Notifications;
using MediatR;

public class GetUnreadNotificationCountQueryHandler(
    IReadDbContext dbContext,
    ICurrentUser currentUser)
    : IRequestHandler<GetUnreadNotificationCountQuery, Result<int>>
{
    public async Task<Result<int>> Handle(
        GetUnreadNotificationCountQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Notifications
            .Where(n => n.UserId == currentUser.UserId.Value && !n.IsRead);
            
        var count = await dbContext.CountAsync(query, cancellationToken);

        return Result<int>.Success(count);
    }
}
