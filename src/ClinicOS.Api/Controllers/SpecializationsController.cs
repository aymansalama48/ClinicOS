namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Contracts.Specializations;
using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Application.Features.Specializations.Commands.CreateSpecialization;
using ClinicOS.Application.Features.Specializations.Commands.DeleteSpecialization;
using ClinicOS.Application.Features.Specializations.Commands.UpdateSpecialization;
using ClinicOS.Application.Features.Specializations.Queries.GetSpecializationById;
using ClinicOS.Application.Features.Specializations.Queries.GetSpecializations;
using ClinicOS.Application.Features.Specializations.Queries.GetSpecializationsLookup;
using ClinicOS.Application.Features.Specializations.Commands.AddSpecializationSchedule;
using ClinicOS.Application.Features.Specializations.Commands.RemoveSpecializationSchedule;
using ClinicOS.Application.Features.Specializations.Commands.ToggleSpecializationStatus;
using ClinicOS.Application.Features.Specializations.Commands.ToggleSpecializationSchedule;
using ClinicOS.Application.Features.Specializations.Commands.UpdateSpecializationSchedule;
using ClinicOS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

[Route("api/[controller]")]
public class SpecializationsController : BaseApiController
{
    [HttpGet("lookup")]
    [Authorize]
    public async Task<IResult> GetLookup([FromQuery] string? searchTerm, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetSpecializationsLookupQuery(searchTerm), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IResult> CreateSpecialization(
        [FromBody] CreateSpecializationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateSpecializationCommand(
            request.Name,
            request.Description,
            request.IconAttachmentId);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IResult> UpdateSpecialization(
        [FromRoute] Guid id,
        [FromBody] UpdateSpecializationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSpecializationCommand(
            id,
            request.Name,
            request.Description,
            request.IconAttachmentId);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    [Authorize]
    public async Task<IResult> GetSpecializations(
        [FromQuery] GetSpecializationsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetSpecializationsQuery(
            request.SearchTerm,
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
    [Authorize]
    public async Task<IResult> GetSpecializationById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetSpecializationByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }


    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IResult> DeleteSpecialization(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeleteSpecializationCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/toggle-status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IResult> ToggleStatus(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new ToggleSpecializationStatusCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/schedules")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IResult> AddSchedule(
        [FromRoute] Guid id,
        [FromBody] AddSpecializationScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddSpecializationScheduleCommand(
            id,
            request.DayOfWeek,
            request.Period,
            request.StartTime,
            request.EndTime);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}/schedules/{scheduleId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IResult> RemoveSchedule(
        [FromRoute] Guid id,
        [FromRoute] Guid scheduleId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveSpecializationScheduleCommand(id, scheduleId);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}/schedules/{scheduleId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IResult> UpdateSchedule(
        [FromRoute] Guid id,
        [FromRoute] Guid scheduleId,
        [FromBody] UpdateSpecializationScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSpecializationScheduleCommand(
            id,
            scheduleId,
            request.DayOfWeek,
            request.Period,
            request.StartTime,
            request.EndTime);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/schedules/{scheduleId:guid}/toggle-status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IResult> ToggleScheduleStatus(
        [FromRoute] Guid id,
        [FromRoute] Guid scheduleId,
        CancellationToken cancellationToken)
    {
        var command = new ToggleSpecializationScheduleCommand(id, scheduleId);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}