using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Domain.Common.Results;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Patients.Queries.GetMyProfile;

public sealed class GetMyPatientProfileQueryHandler : IQueryHandler<GetMyPatientProfileQuery, PatientProfileResponse>
{
    private readonly ICurrentUser _currentUser;
    private readonly IReadDbContext _context;

    public GetMyPatientProfileQueryHandler(ICurrentUser currentUser, IReadDbContext context)
    {
        _currentUser = currentUser;
        _context = context;
    }

    public async Task<Result<PatientProfileResponse>> Handle(GetMyPatientProfileQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue || _currentUser.UserId.Value == Guid.Empty)
        {
            return Result<PatientProfileResponse>.Failure(UserErrors.NotFound);
        }

        var patientId = _currentUser.UserId.Value;

        // ?? ÇÓÊÎÏÇã ÏæÇá IReadDbContext ÇáãÎÕÕÉ ÇáÎÇÕÉ Èß
        var query = _context.Patients.Where(p => p.Id == patientId && !p.IsDeleted);
        var noTrackingQuery = _context.AsNoTracking(query);

        var patient = await noTrackingQuery.FirstOrDefaultAsync(cancellationToken);

        if (patient is null)
        {
            return Result<PatientProfileResponse>.Failure(UserErrors.NotFound);
        }

        var response = new PatientProfileResponse(
            patient.Id,
            patient.FirstName,
            patient.MiddleName,
            patient.LastName,
            patient.FullName,
            patient.PhoneNumber,
            patient.DateOfBirth,
            patient.Gender,
            patient.BloodType,
            patient.EmergencyContact,
            patient.IsAccountLinked
        );

        return Result<PatientProfileResponse>.Success(response);
    }
}
