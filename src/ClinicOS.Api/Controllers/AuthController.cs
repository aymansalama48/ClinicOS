namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Contracts.Accounts;
using ClinicOS.Api.Controllers.Base;
using ClinicOS.Api.Extensions;
using ClinicOS.Application.Features.Accounts.Authentication.Commands.Logout;
using ClinicOS.Application.Features.Accounts.Authentication.Commands.RefreshToken;
using ClinicOS.Application.Features.Accounts.StaffAuth.Commands.StaffGoogleLogin;
using ClinicOS.Application.Features.Accounts.StaffAuth.Commands.StaffLogin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    [HttpPost("staff/login")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingExtensions.LoginPolicy)]
    public async Task<IResult> StaffLogin(
        [FromBody] StaffLoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new StaffLoginCommand(request.Email, request.Password);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("staff/google-login")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingExtensions.LoginPolicy)]
    public async Task<IResult> StaffGoogleLogin(
        [FromBody] StaffGoogleLoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new StaffGoogleLoginCommand(request.IdToken);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingExtensions.RefreshPolicy)]
    public async Task<IResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IResult> Logout(
        [FromBody] LogoutRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LogoutCommand(request.RefreshToken);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
