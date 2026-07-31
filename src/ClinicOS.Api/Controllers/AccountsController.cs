namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ChangePassword;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ForgotPassword;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ResetPassword;
using ClinicOS.Application.Features.Accounts.Authentication.Commands.Logout;
using ClinicOS.Application.Features.Accounts.Authentication.Commands.RefreshToken;
using ClinicOS.Application.Features.Accounts.StaffAuth.Commands.StaffGoogleLogin;
using ClinicOS.Application.Features.Accounts.StaffAuth.Commands.StaffLogin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class AccountsController : BaseApiController
{
    /// <summary>
    /// تسجيل دخول الموظفين (Admin / Doctor / Receptionist) عبر البريد وكلمة السر
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IResult> Login(
        [FromBody] StaffLoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// تسجيل دخول الموظفين عبر Google OAuth
    /// </summary>
    [HttpPost("google-login")]
    [AllowAnonymous]
    public async Task<IResult> GoogleLogin(
        [FromBody] StaffGoogleLoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// تجديد الـ Access Token باستخدام الـ Refresh Token
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IResult> RefreshToken(
        [FromBody] RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// تسجيل الخروج وإبطال الـ Refresh Token
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IResult> Logout(
        [FromBody] LogoutCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// طلب رابط إعادة تعيين كلمة المرور (نسيت كلمة المرور)
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// إعادة تعيين كلمة المرور باستخدام الـ Token
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IResult> ResetPassword(
        [FromBody] ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// تغيير كلمة المرور للمستخدم المسجل حالياً
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IResult> ChangePassword(
        [FromBody] ChangePasswordCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}