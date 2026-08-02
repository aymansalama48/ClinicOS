using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Receptionists; // 👈 استدعاء الأخطاء المركزية
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Receptionists.Commands.DeleteReceptionist;

public sealed class DeleteReceptionistCommandHandler : ICommandHandler<DeleteReceptionistCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserManagementService _userService;

    public DeleteReceptionistCommandHandler(IApplicationDbContext context, IUserManagementService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<Result<bool>> Handle(DeleteReceptionistCommand request, CancellationToken cancellationToken)
    {
        var receptionist = await _context.FirstOrDefaultAsync(
            _context.Receptionists.Where(r => r.Id == request.Id), cancellationToken);

        // استخدام الـ Error المركزي
        if (receptionist is null) return Result<bool>.Failure(ReceptionistErrors.NotFound);

        _context.Remove(receptionist);
        await _context.SaveChangesAsync(cancellationToken);

        // تمرير الـ ID لخدمة إيقاف الحساب
        await _userService.DeactivateUserAsync(receptionist.ApplicationUserId, cancellationToken);

        return Result<bool>.Success(true);
    }
}