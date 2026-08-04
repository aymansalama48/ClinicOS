namespace ClinicOS.Application.Features.Notifications.Commands.MarkNotificationRead;

using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Notifications;
using MediatR;

public class MarkNotificationReadCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUser currentUser)
    : IRequestHandler<MarkNotificationReadCommand, Result>
{
    public async Task<Result> Handle(
        MarkNotificationReadCommand request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Notifications
            .Where(n => n.Id == request.NotificationId);
            
        var notification = await dbContext.FirstOrDefaultAsync(query, cancellationToken);

        if (notification is null || notification.UserId != currentUser.UserId.Value)
        {
            return Result.Failure(new Error("Notification.NotFound", "Notification not found.", ErrorType.NotFound));
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
