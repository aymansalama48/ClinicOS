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
using ClinicOS.Application.Features.Patients.Queries.GetPatientsLookup;
using ClinicOS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

[Route("api/[controller]")]
public class PatientsController : BaseApiController
{
    [HttpGet("lookup")]
    [Authorize]
    public async Task<IResult> GetLookup([FromQuery] string? searchTerm, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetPatientsLookupQuery(searchTerm), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin + "," + Roles.Receptionist)]
    public async Task<IResult> CreatePatient(
        [FromBody] CreatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePatientCommand(
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.PhoneNumber,
            request.DateOfBirth,
            request.Gender,
            request.BloodType,
            request.EmergencyContact,
            request.ApplicationUserId);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Receptionist)]
    public async Task<IResult> UpdatePatient(
        [FromRoute] Guid id,
        [FromBody] UpdatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePatientCommand(
            id,
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.PhoneNumber,
            request.DateOfBirth,
            request.Gender,
            request.BloodType,
            request.EmergencyContact,
            request.ApplicationUserId);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    [Authorize]
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
    [Authorize]
    public async Task<IResult> GetPatientById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetPatientByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
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
    [Authorize(Roles = Roles.Patient)] // 👈 لازم يكون مسجل دخول (Patient Token)
    public async Task<IResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetMyPatientProfileQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// تحديث البروفايل الشخصي للمريض (بناءً على التوكن الخاص به)
    /// </summary>
    [HttpPut("me")]
    [Authorize(Roles = Roles.Patient)] // 👈 لازم يكون مسجل دخول
    public async Task<IResult> UpdateMyProfile(
        [FromBody] UpdateMyPatientProfileRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMyPatientProfileCommand(
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.PhoneNumber,
            request.DateOfBirth,
            request.Gender,
            request.BloodType,
            request.EmergencyContact);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}