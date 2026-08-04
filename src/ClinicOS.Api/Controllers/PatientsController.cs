namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Contracts.Patients; // 👈 استدعاء الـ Contract
using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Application.Features.Patients.Commands.CreatePatient;
using ClinicOS.Application.Features.Patients.Commands.DeletePatient;
using ClinicOS.Application.Features.Patients.Commands.UpdateMyProfile;
using ClinicOS.Application.Features.Patients.Commands.UpdatePatient;
using ClinicOS.Application.Features.Patients.Queries.GetMyProfile;
using ClinicOS.Application.Features.Patients.Queries.GetPatientById;
using ClinicOS.Application.Features.Patients.Queries.GetPatients;
using Microsoft.AspNetCore.Authorization;
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
    // ==========================================================
    // 👇 دوال الخدمة الذاتية للمريض (Self-Service Profile) 👇
    // ==========================================================

    /// <summary>
    /// عرض البروفايل الشخصي للمريض (بناءً على التوكن الخاص به)
    /// </summary>
    [HttpGet("me")]
    [Authorize] // 👈 لازم يكون مسجل دخول (Patient Token)
    public async Task<IResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetMyPatientProfileQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// تحديث البروفايل الشخصي للمريض (بناءً على التوكن الخاص به)
    /// </summary>
    [HttpPut("me")]
    [Authorize] // 👈 لازم يكون مسجل دخول
    public async Task<IResult> UpdateMyProfile(
        [FromBody] UpdateMyPatientProfileCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}