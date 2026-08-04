namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Contracts.PatientAuth;
using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Features.Accounts.PatientAuth.Commands.PatientEmailLogin;
using ClinicOS.Application.Features.Accounts.PatientAuth.Commands.PatientGoogleLogin;
using ClinicOS.Application.Features.Accounts.PatientAuth.Commands.PatientOtpLogin;
using ClinicOS.Application.Features.Accounts.PatientAuth.Commands.RegisterPermanentAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class PatientAuthController : BaseApiController
{
    /// <summary>
    /// تسجيل دخول المريض عبر البريد وكلمة السر
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IResult> Login(
        [FromBody] PatientEmailLoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new PatientEmailLoginCommand(request.Email, request.Password);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// تسجيل دخول المريض عبر Google OAuth
    /// </summary>
    [HttpPost("google-login")]
    [AllowAnonymous]
    public async Task<IResult> GoogleLogin(
        [FromBody] PatientGoogleLoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new PatientGoogleLoginCommand(request.IdToken);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// إنشاء حساب دائم جديد للمريض
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IResult> Register(
        [FromBody] RegisterPermanentAccountRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterPermanentAccountCommand(
            request.Email,
            request.Password,
            request.PhoneNumber,
            request.OtpCode);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// التحقق من رمز OTP وتأكيد دخول المريض
    /// </summary>
    [HttpPost("otp-login")]
    [AllowAnonymous]
    public async Task<IResult> OtpLogin(
        [FromBody] PatientOtpLoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new PatientOtpLoginCommand(
            request.PhoneNumber,
            request.Code,
            request.Purpose);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}