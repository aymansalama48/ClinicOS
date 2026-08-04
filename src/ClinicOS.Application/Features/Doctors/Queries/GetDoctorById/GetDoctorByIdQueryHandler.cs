using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Errors.Doctors;
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctorById;

public sealed class GetDoctorByIdQueryHandler
    : IQueryHandler<GetDoctorByIdQuery, DoctorDetailsResponse>
{
    private readonly IReadDbContext _context;
    private readonly IUserManagementService _userService;

    public GetDoctorByIdQueryHandler(
        IReadDbContext context,
        IUserManagementService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<Result<DoctorDetailsResponse>> Handle(
        GetDoctorByIdQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Ã·» »Ì«‰«  «·ÿ»Ì» ﬂ‹ Entity
        var query = _context.Doctors.Where(d => d.Id == request.Id);
        var noTrackingQuery = _context.AsNoTracking(query);
        var doctor = await noTrackingQuery.FirstOrDefaultAsync(cancellationToken);

        if (doctor is null)
        {
            return Result<DoctorDetailsResponse>.Failure(DoctorErrors.NotFound);
        }

        // 2. Ã·» »Ì«‰«  «·„” Œœ„ «·„— »ÿ »«·ÿ»Ì»
        var userResult = await _userService.GetByIdAsync(doctor.ApplicationUserId, cancellationToken);
        var user = userResult.IsSuccess ? userResult.Data : null;

        // 3. œ„Ã «·»Ì«‰«  Ê≈—Ã«⁄ «·‹ DTO
        var response = new DoctorDetailsResponse(
            doctor.Id,
            doctor.SpecializationId,
            doctor.ApplicationUserId,
            user?.FullName ?? "„” Œœ„ €Ì— „⁄—Ê›",
            user?.Email,
            user?.AvatarUrl,
            doctor.Bio,
            doctor.YearsOfExperience,
            doctor.ConsultationFee,
            doctor.UrgentSurchargeFee,
            doctor.IsActive
        );

        return Result<DoctorDetailsResponse>.Success(response);
    }
}
