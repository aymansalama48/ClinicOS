namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Features.Accounts.StaffInvitations.Commands.AcceptInvitation;
using ClinicOS.Application.Features.Accounts.StaffInvitations.Commands.AcceptInvitationWithGoogle;
using ClinicOS.Application.Features.Accounts.StaffInvitations.Commands.SendInvitation;
using ClinicOS.Application.Features.Accounts.StaffInvitations.Queries.ValidateInvitation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Route("api/staff-invitations")]
public class StaffInvitationsController : BaseApiController
{
    /// <summary>
    /// إرسال دعوة لموظف جديد (خاص بالأدمن فقط)
    /// </summary>
    [HttpPost("send")]
    //[Authorize(Roles = "Admin")]
    public async Task<IResult> SendInvitation(
        [FromBody] SendStaffInvitationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// التحقق من صلاحية التوكن (يُستدعى من الـ Frontend عند فتح صفحة قبول الدعوة)
    /// </summary>
    [HttpGet("validate/{token}")]
    [AllowAnonymous]
    public async Task<IResult> ValidateInvitation(
        string token,
        CancellationToken cancellationToken)
    {
        var query = new ValidateInvitationQuery(token);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// قبول الدعوة وإنشاء الحساب (بالطريقة التقليدية: إيميل وباسورد)
    /// </summary>
    [HttpPost("accept")]
    [AllowAnonymous]
    public async Task<IResult> AcceptInvitation(
        [FromBody] AcceptInvitationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// قبول الدعوة وإنشاء الحساب (عبر جوجل)
    /// </summary>
    [HttpPost("accept-google")]
    [AllowAnonymous]
    public async Task<IResult> AcceptInvitationWithGoogle(
        [FromBody] AcceptInvitationWithGoogleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}