using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Patients; // 👈 استدعاء الأخطاء المركزية
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Patients.Commands.DeletePatient;

public sealed class DeletePatientCommandHandler : ICommandHandler<DeletePatientCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserManagementService _userService;

    public DeletePatientCommandHandler(IApplicationDbContext context, IUserManagementService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<Result<bool>> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _context.FirstOrDefaultAsync(
            _context.Patients.Where(p => p.Id == request.Id), cancellationToken);

        // استخدام الـ Error المركزي
        if (patient is null) return Result<bool>.Failure(PatientErrors.NotFound);

        _context.Remove(patient);
        await _context.SaveChangesAsync(cancellationToken);

        // 👇 الحل المعماري الصحيح للـ Nullable Guid
        if (patient.ApplicationUserId.HasValue)
        {
            await _userService.DeactivateUserAsync(patient.ApplicationUserId.Value, cancellationToken);
        }

        return Result<bool>.Success(true);
    }
}