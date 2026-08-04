using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Errors.Patients;
using ClinicOS.Domain.Common.Results;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Patients.Commands.UpdatePatient;

public sealed class UpdatePatientCommandHandler : ICommandHandler<UpdatePatientCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public UpdatePatientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        // 1. Ã·» «·„—Ì÷ ·· ⁄œÌ· (Tracking «·⁄«œÌ)
        var query = _context.Patients.Where(p => p.Id == request.PatientId);
        var patient = await query.FirstOrDefaultAsync(cancellationToken);

        if (patient is null)
        {
            return Result<Guid>.Failure(PatientErrors.NotFound);
        }

        // 2. «· Õﬁﬁ „‰ ⁄œ„ «” Œœ«„ —ﬁ„ «·Â« › ·„—Ì÷ *¬Œ—*
        var phoneQuery = _context.Patients
            .Where(p => p.PhoneNumber == request.PhoneNumber && p.Id != request.PatientId);
        var isPhoneExists = await phoneQuery.AnyAsync(cancellationToken);

        if (isPhoneExists)
        {
            return Result<Guid>.Failure(PatientErrors.PhoneNumberAlreadyExists);
        }

        // 3.  ÕœÌÀ «·»Ì«‰« 
        patient.FirstName = request.FirstName;
        patient.MiddleName = request.MiddleName;
        patient.LastName = request.LastName;
        patient.PhoneNumber = request.PhoneNumber;
        patient.DateOfBirth = request.DateOfBirth;
        patient.Gender = request.Gender;
        patient.BloodType = request.BloodType;
        patient.EmergencyContact = request.EmergencyContact;
        patient.ApplicationUserId = request.ApplicationUserId;

        // 4. Õ›Ÿ «· ⁄œÌ·« 
        _context.Patients.  Update(patient);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(patient.Id);
    }
}
