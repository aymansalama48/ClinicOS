using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Receptionists;
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Receptionists.Queries.GetMyProfile;

public sealed class GetMyReceptionistProfileQueryHandler : IQueryHandler<GetMyReceptionistProfileQuery, ReceptionistProfileResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetMyReceptionistProfileQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<ReceptionistProfileResponse>> Handle(GetMyReceptionistProfileQuery request, CancellationToken cancellationToken)
    {
        var receptionist = await _context.FirstOrDefaultAsync(
            _context.AsNoTracking(_context.Receptionists).Where(r => r.ApplicationUserId == _currentUser.UserId),
            cancellationToken);

        if (receptionist is null) return Result<ReceptionistProfileResponse>.Failure(ReceptionistErrors.ProfileNotFound);

        return Result<ReceptionistProfileResponse>.Success(new ReceptionistProfileResponse(
            receptionist.Id,
            receptionist.SpecializationId,
            receptionist.IsActive
        ));
    }
}