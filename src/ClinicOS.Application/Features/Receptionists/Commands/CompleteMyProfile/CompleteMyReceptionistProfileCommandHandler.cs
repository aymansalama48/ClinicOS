using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Receptionists;
using ClinicOS.Application.Common.Errors.Users; // 👈 ضفنا ده عشان UserErrors
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Receptionists;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Receptionists.Commands.CompleteMyProfile;

public sealed class CompleteMyReceptionistProfileCommandHandler : ICommandHandler<CompleteMyReceptionistProfileCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CompleteMyReceptionistProfileCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CompleteMyReceptionistProfileCommand request, CancellationToken cancellationToken)
    {
        // 👇 التأكد من إن الـ ID موجود ومفيش فيه مشكلة
        if (!_currentUser.UserId.HasValue || _currentUser.UserId.Value == Guid.Empty)
        {
            return Result<Guid>.Failure(UserErrors.NotFound);
        }

        var userId = _currentUser.UserId.Value;

        // استخدمنا المتغير الجديد (userId) بدل _currentUser.UserId
        var profileExists = await _context.AnyAsync(
            _context.Receptionists.Where(r => r.ApplicationUserId == userId), cancellationToken);

        if (profileExists) return Result<Guid>.Failure(ReceptionistErrors.ProfileAlreadyExists);

        var receptionist = Receptionist.Create(userId, request.SpecializationId);

        _context.Add(receptionist);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(receptionist.Id);
    }
}