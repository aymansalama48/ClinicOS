using ClinicOS.Application.Features.Dashboards.Queries.GetAdminDashboardSummary;
using ClinicOS.Application.Features.Dashboards.Queries.GetDoctorDashboardSummary;
using ClinicOS.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DashboardsController(IMediator mediator) : ControllerBase
{
    [HttpGet("admin")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetAdminDashboard()
    {
        var result = await mediator.Send(new GetAdminDashboardSummaryQuery());
        return Ok(result);
    }

    [HttpGet("doctor/{doctorId}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Doctor)]
    public async Task<IActionResult> GetDoctorDashboard(Guid doctorId)
    {
        var result = await mediator.Send(new GetDoctorDashboardSummaryQuery(doctorId));
        return Ok(result);
    }
}
