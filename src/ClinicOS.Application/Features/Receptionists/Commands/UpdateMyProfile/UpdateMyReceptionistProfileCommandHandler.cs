using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Receptionists;
using ClinicOS.Domain.Common.Results;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Receptionists.Commands.UpdateMyProfile;

public sealed class UpdateMyReceptionistProfileCommandHandler : ICommandHandler<UpdateMyReceptionistProfileCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public UpdateMyReceptionistProfileCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(UpdateMyReceptionistProfileCommand request, CancellationToken cancellationToken)
    {
        var receptionist = await _context.FirstOrDefaultAsync(
            _context.Receptionists.Where(r => r.ApplicationUserId == _currentUser.UserId), cancellationToken);

        if (receptionist is null) return Result<Guid>.Failure(ReceptionistErrors.ProfileNotFound);

        receptionist.UpdateProfile(request.SpecializationId);

        _context.Update(receptionist);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(receptionist.Id);
    }
}