namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Features.Otps.Commands.RequestOtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class OtpsController : BaseApiController
{
    /// <summary>
    /// طلب إرسال كود OTP جديد برقم الهاتف
    /// </summary>
    [HttpPost("request")]
    [AllowAnonymous]
    public async Task<IResult> RequestOtp(
        [FromBody] RequestOtpCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}