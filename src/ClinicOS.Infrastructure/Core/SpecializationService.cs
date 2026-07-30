using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Core;

public class SpecializationService : ISpecializationService
{
    private readonly AppDbContext _context;

    public SpecializationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid?> GetSpecializationIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // 1. البحث في جدول الأطباء
        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(d => d.ApplicationUserId == userId && !d.IsDeleted, cancellationToken);

        if (doctor is not null)
            return doctor.SpecializationId;

        // 2. البحث في جدول موظفي الاستقبال
        var receptionist = await _context.Receptionists
            .FirstOrDefaultAsync(r => r.ApplicationUserId == userId && !r.IsDeleted, cancellationToken);

        return receptionist?.SpecializationId;
    }
}