using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Patients;
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Patients.Queries.GetPatientById;

public sealed class GetPatientByIdQueryHandler : IQueryHandler<GetPatientByIdQuery, PatientDetailsResponse>
{
    private readonly IApplicationDbContext _context;

    public GetPatientByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PatientDetailsResponse>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Patients.Where(p => p.Id == request.Id);
        var noTrackingQuery = _context.AsNoTracking(query);

        var patient = await _context.FirstOrDefaultAsync(
            noTrackingQuery.Select(p => new PatientDetailsResponse(
                p.Id,
                p.FirstName,
                p.MiddleName,
                p.LastName,
                string.IsNullOrEmpty(p.MiddleName) ? p.FirstName + " " + p.LastName : p.FirstName + " " + p.MiddleName + " " + p.LastName,
                p.PhoneNumber,
                p.DateOfBirth,
                p.Gender,
                p.BloodType,
                p.EmergencyContact,
                p.ApplicationUserId,
                p.ApplicationUserId != null
            )),
            cancellationToken);

        if (patient is null)
        {
            return Result<PatientDetailsResponse>.Failure(PatientErrors.NotFound);
        }

        return Result<PatientDetailsResponse>.Success(patient);
    }
}