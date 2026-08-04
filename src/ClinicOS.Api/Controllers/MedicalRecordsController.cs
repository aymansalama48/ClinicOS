using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Features.MedicalRecords.Commands.AddPrescriptionToRecord;
using ClinicOS.Application.Features.MedicalRecords.Commands.AddTestToRecord;
using ClinicOS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Api.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = Roles.Doctor)]
public class MedicalRecordsController : BaseApiController
{
    [HttpPost("{id:guid}/prescriptions")]
    public async Task<IResult> AddPrescription(
        [FromRoute] Guid id,
        [FromBody] AddPrescriptionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddPrescriptionToRecordCommand(id, request.Medications, request.Notes);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/tests")]
    public async Task<IResult> AddTest(
        [FromRoute] Guid id,
        [FromBody] AddTestRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddTestToRecordCommand(id, request.TestName, request.ResultDescription);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}

public class AddPrescriptionRequest
{
    public string Medications { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class AddTestRequest
{
    public string TestName { get; set; } = string.Empty;
    public string ResultDescription { get; set; } = string.Empty;
}
