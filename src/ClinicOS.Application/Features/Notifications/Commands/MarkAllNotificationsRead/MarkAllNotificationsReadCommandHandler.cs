namespace ClinicOS.Application.Features.Notifications.Commands.MarkAllNotificationsRead;

using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Notifications;
using MediatR;

public class MarkAllNotificationsReadCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUser currentUser)
    : IRequestHandler<MarkAllNotificationsReadCommand, Result>
{
    public async Task<Result> Handle(
        MarkAllNotificationsReadCommand request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Notifications
            .Where(n => n.UserId == currentUser.UserId.Value && !n.IsRead);
            
        var notifications = await dbContext.ToListAsync(query, cancellationToken);

        if (notifications.Count != 0)
        {
            foreach (var n in notifications)
            {
                n.IsRead = true;
            }
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
