using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Doctors;
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Doctors.Commands.DeleteDoctor;

public sealed class DeleteDoctorCommandHandler : ICommandHandler<DeleteDoctorCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserManagementService _userService;

    public DeleteDoctorCommandHandler(IApplicationDbContext context, IUserManagementService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<Result<bool>> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _context.FirstOrDefaultAsync(
            _context.Doctors.Where(d => d.Id == request.Id), cancellationToken);

        if (doctor is null) return Result<bool>.Failure(DoctorErrors.NotFound);

        _context.Remove(doctor);
        await _context.SaveChangesAsync(cancellationToken);

        await _userService.DeactivateUserAsync(doctor.ApplicationUserId, cancellationToken);

        return Result<bool>.Success(true);
    }
}