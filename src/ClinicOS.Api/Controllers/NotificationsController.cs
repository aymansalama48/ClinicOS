namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Features.Notifications.Commands.MarkAllNotificationsRead;
using ClinicOS.Application.Features.Notifications.Commands.MarkNotificationRead;
using ClinicOS.Application.Features.Notifications.Queries.GetMyNotifications;
using ClinicOS.Application.Features.Notifications.Queries.GetUnreadNotificationCount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[Authorize]
public class NotificationsController : BaseApiController
{
    [HttpGet("me")]
    public async Task<IResult> GetMyNotifications(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? unreadOnly = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMyNotificationsQuery(pageNumber, pageSize, unreadOnly);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("me/unread-count")]
    public async Task<IResult> GetUnreadCount(CancellationToken cancellationToken)
    {
        var query = new GetUnreadNotificationCountQuery();
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}/read")]
    public async Task<IResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        var command = new MarkNotificationReadCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("me/read-all")]
    public async Task<IResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var command = new MarkAllNotificationsReadCommand();
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
