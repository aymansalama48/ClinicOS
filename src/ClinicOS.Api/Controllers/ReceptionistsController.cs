namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Contracts.Receptionists; // 👈 استدعاء الـ Contract
using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Application.Features.Receptionists.Commands.CompleteMyProfile;
using ClinicOS.Application.Features.Receptionists.Commands.DeleteReceptionist;
using ClinicOS.Application.Features.Receptionists.Commands.UpdateMyProfile;
using ClinicOS.Application.Features.Receptionists.Queries.GetMyProfile;
using ClinicOS.Application.Features.Receptionists.Queries.GetReceptionistById;
using ClinicOS.Application.Features.Receptionists.Queries.GetReceptionists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

[Route("api/[controller]")]
public class ReceptionistsController : BaseApiController
{

    [HttpGet]
    public async Task<IResult> GetReceptionists(
        [FromQuery] GetReceptionistsRequest request, // 👈 استخدام الـ Request الشيك
        CancellationToken cancellationToken)
    {
        // تحويل الـ API Contract لـ Application Query
        var query = new GetReceptionistsQuery(
            request.SpecializationId,
            request.IsActive,
            new PaginationParameters
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            }
        );

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IResult> GetReceptionistById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetReceptionistByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }
    [HttpDelete("{id:guid}")]
    public async Task<IResult> DeleteReceptionist(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeleteReceptionistCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("me/profile")]
    [Authorize(Roles = "Receptionist")]
    public async Task<IResult> CompleteMyProfile(
        [FromBody] CompleteMyReceptionistProfileRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CompleteMyReceptionistProfileCommand(request.SpecializationId);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("me/profile")]
    [Authorize(Roles = "Receptionist")]
    public async Task<IResult> UpdateMyProfile(
        [FromBody] UpdateMyReceptionistProfileRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMyReceptionistProfileCommand(request.SpecializationId);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("me/profile")]
    [Authorize(Roles = "Receptionist")]
    public async Task<IResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetMyReceptionistProfileQuery(), cancellationToken);
        return HandleResult(result);
    }
}