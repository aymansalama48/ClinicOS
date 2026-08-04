using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Errors.Patients; // ?? «” œ⁄«¡ «·√Œÿ«¡ «·„—ﬂ“Ì…
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
        var patient = await 
            _context.Patients.Where(p => p.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

        // «” Œœ«„ «·‹ Error «·„—ﬂ“Ì
        if (patient is null) return Result<bool>.Failure(PatientErrors.NotFound);

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync(cancellationToken);

        // ?? «·Õ· «·„⁄„«—Ì «·’ÕÌÕ ··‹ Nullable Guid
        if (patient.ApplicationUserId.HasValue)
        {
            await _userService.DeactivateUserAsync(patient.ApplicationUserId.Value, cancellationToken);
        }

        return Result<bool>.Success(true);
    }
}
