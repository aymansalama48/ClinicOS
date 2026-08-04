using MediatR;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Application.Features.Dashboards.Queries.GetDoctorDashboardSummary;

public record GetDoctorDashboardSummaryQuery(Guid DoctorId) : IRequest<DoctorDashboardSummaryVm>;

public class DoctorDashboardSummaryVm
{
    public int TotalPatients { get; set; }
    public int TodayAppointments { get; set; }
    public int PendingAppointments { get; set; }
    public int CompletedAppointments { get; set; }
}

public class GetDoctorDashboardSummaryQueryHandler(IReadDbContext context) : IRequestHandler<GetDoctorDashboardSummaryQuery, DoctorDashboardSummaryVm>
{
    public async Task<DoctorDashboardSummaryVm> Handle(GetDoctorDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var todayAppointments = context.Appointments
            .Where(a => a.DoctorId == request.DoctorId && a.AppointmentDate == today);
            
        var pendingAppointments = context.Appointments
            .Where(a => a.DoctorId == request.DoctorId && a.Status == ClinicOS.Domain.Enums.AppointmentStatus.Pending);

        var completedAppointments = context.Appointments
            .Where(a => a.DoctorId == request.DoctorId && a.Status == ClinicOS.Domain.Enums.AppointmentStatus.Completed);

        var totalPatients = await context.Appointments
            .Where(a => a.DoctorId == request.DoctorId)
            .Select(a => a.PatientId)
            .Distinct()
            .CountAsync(cancellationToken);

        return new DoctorDashboardSummaryVm
        {
            TotalPatients = totalPatients,
            TodayAppointments = await context.CountAsync(todayAppointments, cancellationToken),
            PendingAppointments = await context.CountAsync(pendingAppointments, cancellationToken),
            CompletedAppointments = await context.CountAsync(completedAppointments, cancellationToken)
        };
    }
}
