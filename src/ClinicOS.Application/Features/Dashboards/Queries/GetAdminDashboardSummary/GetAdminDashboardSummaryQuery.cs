using MediatR;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Application.Features.Dashboards.Queries.GetAdminDashboardSummary;

public record GetAdminDashboardSummaryQuery : IRequest<AdminDashboardSummaryVm>;

public class AdminDashboardSummaryVm
{
    public int TotalPatients { get; set; }
    public decimal TodayRevenue { get; set; }
    public Dictionary<string, int> AppointmentsByStatus { get; set; } = new();
    public int ActiveDoctors { get; set; }
}

public class GetAdminDashboardSummaryQueryHandler(IReadDbContext context) : IRequestHandler<GetAdminDashboardSummaryQuery, AdminDashboardSummaryVm>
{
    public async Task<AdminDashboardSummaryVm> Handle(GetAdminDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var todayRevenue = await context.Appointments
            .Where(a => a.AppointmentDate == today && a.Status == ClinicOS.Domain.Enums.AppointmentStatus.Completed)
            .SumAsync(a => a.TotalAmount ?? 0, cancellationToken);

        var appointments = await context.Appointments.ToListAsync(cancellationToken);
        
        var appointmentsByStatus = appointments
            .GroupBy(a => a.Status.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var activeDoctors = await context.Doctors
            .Where(d => d.IsActive)
            .CountAsync(cancellationToken);

        return new AdminDashboardSummaryVm
        {
            TotalPatients = await context.CountAsync(context.Patients, cancellationToken),
            TodayRevenue = todayRevenue,
            AppointmentsByStatus = appointmentsByStatus,
            ActiveDoctors = activeDoctors
        };
    }
}
