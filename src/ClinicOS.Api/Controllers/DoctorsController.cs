namespace ClinicOS.Api.Controllers;

using ClinicOS.Api.Contracts.Doctors; // 👈 استدعاء الـ Contracts
using ClinicOS.Api.Controllers.Base;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Application.Features.Doctors.Commands.CompleteMyProfile;
using ClinicOS.Application.Features.Doctors.Commands.DeleteDoctor;
using ClinicOS.Application.Features.Doctors.Commands.SetDoctorAvailability;
using ClinicOS.Application.Features.Doctors.Commands.UpdateMyProfile;
using ClinicOS.Application.Features.Doctors.Queries.GetDoctorAvailabilities;
using ClinicOS.Application.Features.Doctors.Queries.GetDoctorById;
using ClinicOS.Application.Features.Doctors.Queries.GetDoctors;
using ClinicOS.Application.Features.Doctors.Queries.GetMyProfile;
using ClinicOS.Application.Features.Doctors.Queries.GetDoctorsLookup;
using ClinicOS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

[Route("api/[controller]")]
public class DoctorsController : BaseApiController
{

    [HttpGet("lookup")]
    [Authorize]
    public async Task<IResult> GetLookup([FromQuery] string? searchTerm, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetDoctorsLookupQuery(searchTerm), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/availability")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Doctor)]
    public async Task<IResult> SetDoctorAvailability(
        Guid id,
        [FromBody] SetDoctorAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SetDoctorAvailabilityCommand(
            id,
            request.DayOfWeek,
            request.Period,
            request.StartTime,
            request.EndTime,
            request.MaxPatients);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    [Authorize]
    public async Task<IResult> GetDoctors(
        [FromQuery] GetDoctorsRequest request, // 👈 استخدام الـ Request الشيك
        CancellationToken cancellationToken)
    {
        // تحويل الـ API Contract لـ Application Query
        var query = new GetDoctorsQuery(
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
    [Authorize]
    public async Task<IResult> GetDoctorById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetDoctorByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}/availabilities")]
    [Authorize]
    public async Task<IResult> GetDoctorAvailabilities(
            Guid id,
            [FromQuery] GetDoctorAvailabilitiesRequest request,
            CancellationToken cancellationToken)
    {
        // 👈 دلوقتي الـ Query بياخد 3 بارامترات، وكله متربط ببعضه
        var query = new GetDoctorAvailabilitiesQuery(
            id,
            request.DayOfWeek,
            new PaginationParameters
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            }
        );

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IResult> DeleteDoctor(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeleteDoctorCommand(id), cancellationToken);
        return HandleResult(result);
    }


    [HttpPost("me/profile")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<IResult> CompleteMyProfile(
        [FromBody] CompleteMyDoctorProfileRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CompleteMyDoctorProfileCommand(
            request.SpecializationId,
            request.Bio,
            request.YearsOfExperience,
            request.ConsultationFee,
            request.UrgentSurchargeFee);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("me/profile")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<IResult> UpdateMyProfile(
        [FromBody] UpdateMyDoctorProfileRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMyDoctorProfileCommand(
            request.SpecializationId,
            request.Bio,
            request.YearsOfExperience,
            request.ConsultationFee,
            request.UrgentSurchargeFee);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("me/profile")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<IResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetMyDoctorProfileQuery(), cancellationToken);
        return HandleResult(result);
    }
}