using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Features.Accounts.Commands.ChangePassword;
using ClinicOS.Application.Features.Accounts.Commands.ForgotPassword;
using ClinicOS.Application.Features.Accounts.Commands.ResetPassword;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOS.Api.Controllers;

public class AccountsController : BaseApiController
{
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