using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Doctors;
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctorById;

public sealed class GetDoctorByIdQueryHandler
    : IQueryHandler<GetDoctorByIdQuery, DoctorDetailsResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserManagementService _userService;

    public GetDoctorByIdQueryHandler(
        IApplicationDbContext context,
        IUserManagementService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<Result<DoctorDetailsResponse>> Handle(
        GetDoctorByIdQuery request,
        CancellationToken cancellationToken)
    {
        // 1. جلب بيانات الطبيب كـ Entity
        var query = _context.Doctors.Where(d => d.Id == request.Id);
        var noTrackingQuery = _context.AsNoTracking(query);
        var doctor = await _context.FirstOrDefaultAsync(noTrackingQuery, cancellationToken);

        if (doctor is null)
        {
            return Result<DoctorDetailsResponse>.Failure(DoctorErrors.NotFound);
        }

        // 2. جلب بيانات المستخدم المرتبط بالطبيب
        var userResult = await _userService.GetByIdAsync(doctor.ApplicationUserId, cancellationToken);
        var user = userResult.IsSuccess ? userResult.Data : null;

        // 3. دمج البيانات وإرجاع الـ DTO
        var response = new DoctorDetailsResponse(
            doctor.Id,
            doctor.SpecializationId,
            doctor.ApplicationUserId,
            user?.FullName ?? "مستخدم غير معروف",
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