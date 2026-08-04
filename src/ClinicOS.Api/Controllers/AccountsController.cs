namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Contracts.Accounts;
using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ActivateUser;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.AssignRoleToUser;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ChangePassword;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.DeactivateUser;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ForgotPassword;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.RemoveRoleFromUser;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ResetPassword;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.UpdateMyProfile;
using ClinicOS.Application.Features.Accounts.AccountManagement.Commands.UpdateMyProfilePicture;
using ClinicOS.Application.Features.Accounts.AccountManagement.Queries.GetAllUsers;
using ClinicOS.Application.Features.Accounts.AccountManagement.Queries.GetMyProfile;
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

    [HttpGet("me/profile")]
    [Authorize] // متاح لأي مستخدم مسجل الدخول
    public async Task<IResult> GetMyAccountProfile(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetMyAccountProfileQuery(), cancellationToken);
        return HandleResult(result);
    }
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> GetAllUsers(
        [FromQuery] GetUsersRequest request,
        CancellationToken cancellationToken)
    {
        // Mapping من الـ Contract للـ Query
        var query = new GetAllUsersQuery(
            request.PageNumber,
            request.PageSize,
            request.Role,
            request.SearchTerm);

        var result = await Mediator.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("me/profile")]
    [Authorize]
    public async Task<IResult> UpdateMyAccountProfile(
        [FromBody] UpdateMyAccountProfileCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("me/profile/picture")]
    [Authorize]
    public async Task<IResult> UpdateMyProfilePicture(
        [FromBody] UpdateMyProfilePictureCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
    [HttpPut("{userId:guid}/deactivate")]
    [Authorize(Roles = "Admin")] // 👈 حماية للآدمن فقط
    public async Task<IResult> DeactivateUser(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new DeactivateUserCommand(userId);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{userId:guid}/activate")]
    [Authorize(Roles = "Admin")] // 👈 حماية للآدمن فقط
    public async Task<IResult> ActivateUser(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new ActivateUserCommand(userId);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
    /// <summary>
    /// إضافة دور لمستخدم
    /// </summary>
    [HttpPost("{userId:guid}/roles")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")] // تأكد من وضع الصلاحية المناسبة
    public async Task<IResult> AssignRoleToUser(
        [FromRoute] Guid userId,
        [FromBody] AssignRoleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AssignRoleToUserCommand(userId, request.RoleName);
        var result = await Mediator.Send(command, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// سحب دور من مستخدم
    /// </summary>
    [HttpDelete("{userId:guid}/roles/{roleName}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IResult> RemoveRoleFromUser(
        [FromRoute] Guid userId,
        [FromRoute] string roleName,
        CancellationToken cancellationToken)
    {
        var command = new RemoveRoleFromUserCommand(userId, roleName);
        var result = await Mediator.Send(command, cancellationToken);

        return HandleResult(result);
    }

    // 💡 الكلاس الخاص بالريكويست (ممكن تحطه في ملف الـ Contracts أو جوه الكنترولر)
    public record AssignRoleRequest(string RoleName);
}