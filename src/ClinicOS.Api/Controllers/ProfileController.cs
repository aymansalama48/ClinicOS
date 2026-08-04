namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Contracts.Accounts;
using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.UpdateMyProfile;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.UpdateMyProfilePicture;
using ClinicOS.Application.Features.Accounts.AccountManagement.Queries.GetMyProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class ProfileController : BaseApiController
{
    [HttpGet("me")]
    [Authorize]
    public async Task<IResult> GetMyAccountProfile(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetMyAccountProfileQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<IResult> UpdateMyAccountProfile(
        [FromBody] UpdateMyAccountProfileRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMyAccountProfileCommand(
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.PhoneNumber);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("me/picture")]
    [Authorize]
    public async Task<IResult> UpdateMyProfilePicture(
        [FromBody] UpdateMyProfilePictureRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMyProfilePictureCommand(request.AvatarUrl);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
