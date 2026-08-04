namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Contracts.Accounts;
using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ActivateUser;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.AssignRoleToUser;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.DeactivateUser;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.RemoveRoleFromUser;
using ClinicOS.Application.Features.Accounts.AccountManagement.Queries.GetAllUsers;
using Microsoft.AspNetCore.Authorization;
using ClinicOS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class UsersController : BaseApiController
{
    [HttpGet]
    public async Task<IResult> GetAllUsers(
        [FromQuery] GetUsersRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetAllUsersQuery(
            request.PageNumber,
            request.PageSize,
            request.Role,
            request.SearchTerm);

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{userId:guid}/deactivate")]
    public async Task<IResult> DeactivateUser(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new DeactivateUserCommand(userId);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{userId:guid}/activate")]
    public async Task<IResult> ActivateUser(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new ActivateUserCommand(userId);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{userId:guid}/roles")]
    public async Task<IResult> AssignRoleToUser(
        [FromRoute] Guid userId,
        [FromBody] AssignRoleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AssignRoleToUserCommand(userId, request.RoleName);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{userId:guid}/roles/{roleName}")]
    public async Task<IResult> RemoveRoleFromUser(
        [FromRoute] Guid userId,
        [FromRoute] string roleName,
        CancellationToken cancellationToken)
    {
        var command = new RemoveRoleFromUserCommand(userId, roleName);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
