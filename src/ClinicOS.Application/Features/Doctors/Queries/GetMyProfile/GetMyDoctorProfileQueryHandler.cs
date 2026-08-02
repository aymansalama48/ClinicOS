using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Doctors;
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Doctors.Queries.GetMyProfile;


public sealed class GetMyDoctorProfileQueryHandler : IQueryHandler<GetMyDoctorProfileQuery, DoctorProfileResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetMyDoctorProfileQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<DoctorProfileResponse>> Handle(GetMyDoctorProfileQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _context.FirstOrDefaultAsync(
            _context.AsNoTracking(_context.Doctors).Where(d => d.ApplicationUserId == _currentUser.UserId),
            cancellationToken);

        if (doctor is null) return Result<DoctorProfileResponse>.Failure(DoctorErrors.ProfileNotFound);

        return Result<DoctorProfileResponse>.Success(new DoctorProfileResponse(
            doctor.Id,
            doctor.SpecializationId,
            doctor.Bio,
            doctor.YearsOfExperience,
            doctor.ConsultationFee,
            doctor.UrgentSurchargeFee,
            doctor.IsActive
        ));
    }
}