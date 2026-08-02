namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Contracts.Patients; // 👈 استدعاء الـ Contract
using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Application.Features.Patients.Commands.CreatePatient;
using ClinicOS.Application.Features.Patients.Commands.DeletePatient;
using ClinicOS.Application.Features.Patients.Commands.UpdatePatient;
using ClinicOS.Application.Features.Patients.Queries.GetPatientById;
using ClinicOS.Application.Features.Patients.Queries.GetPatients;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

[Route("api/[controller]")]
public class PatientsController : BaseApiController
{
    [HttpPost]
    public async Task<IResult> CreatePatient(
        [FromBody] CreatePatientCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IResult> UpdatePatient(
        Guid id,
        [FromBody] UpdatePatientCommand command,
        CancellationToken cancellationToken)
    {
        var request = command with { PatientId = id };
        var result = await Mediator.Send(request, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IResult> GetPatients(
        [FromQuery] GetPatientsRequest request, // 👈 استخدام الـ Request الشيك
        CancellationToken cancellationToken)
    {
        // تحويل الـ API Contract لـ Application Query
        var query = new GetPatientsQuery(
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
    public async Task<IResult> GetPatientById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetPatientByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }
    [HttpDelete("{id:guid}")]
    public async Task<IResult> DeletePatient(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeletePatientCommand(id), cancellationToken);
        return HandleResult(result);
    }
}