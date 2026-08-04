namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Contracts.Specializations;
using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Application.Features.Specializations.Commands.CreateSpecialization;
using ClinicOS.Application.Features.Specializations.Commands.DeleteSpecialization;
using ClinicOS.Application.Features.Specializations.Commands.UpdateSpecialization;
using ClinicOS.Application.Features.Specializations.Queries.GetSpecializationById;
using ClinicOS.Application.Features.Specializations.Queries.GetSpecializations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

[Route("api/[controller]")]
public class SpecializationsController : BaseApiController
{
    [HttpPost]
    public async Task<IResult> CreateSpecialization(
        [FromBody] CreateSpecializationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateSpecializationCommand(
            request.Name,
            request.Description);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IResult> UpdateSpecialization(
        [FromRoute] Guid id,
        [FromBody] UpdateSpecializationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSpecializationCommand(
            id,
            request.Name,
            request.Description);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
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
    public async Task<IResult> GetSpecializationById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetSpecializationByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }


    [HttpDelete("{id:guid}")]
    public async Task<IResult> DeleteSpecialization(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeleteSpecializationCommand(id), cancellationToken);
        return HandleResult(result);
    }
}