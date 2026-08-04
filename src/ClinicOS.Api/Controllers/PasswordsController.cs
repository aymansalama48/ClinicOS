namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Contracts.Accounts;
using ClinicOS.Api.Controllers.Base;
using ClinicOS.Api.Extensions;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ChangePassword;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ForgotPassword;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ResetPassword;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[Route("api/[controller]")]
public class PasswordsController : BaseApiController
{
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingExtensions.PasswordResetRequestPolicy)]
    public async Task<IResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ForgotPasswordCommand(request.Email);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingExtensions.PasswordResetRequestPolicy)]
    public async Task<IResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ResetPasswordCommand(
            request.Email,
            request.Token,
            request.NewPassword,
            request.ConfirmPassword);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangePasswordCommand(
            request.CurrentPassword,
            request.NewPassword,
            request.ConfirmNewPassword);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
