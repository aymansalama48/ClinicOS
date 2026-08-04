using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Domain.Common.Results;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Patients.Commands.UpdateMyProfile;

public sealed class UpdateMyPatientProfileCommandHandler : ICommandHandler<UpdateMyPatientProfileCommand, bool>
{
    private readonly ICurrentUser _currentUser;
    private readonly IApplicationDbContext _context;

    public UpdateMyPatientProfileCommandHandler(ICurrentUser currentUser, IApplicationDbContext context)
    {
        _currentUser = currentUser;
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateMyPatientProfileCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
        {
            return Result<bool>.Failure(UserErrors.NotFound);
        }

        var patientId = _currentUser.UserId.Value;

        // 👈 استخدام دوال IApplicationDbContext المخصصة
        var query = _context.Patients.Where(p => p.Id == patientId && !p.IsDeleted);
        var patient = await _context.FirstOrDefaultAsync(query, cancellationToken);

        if (patient is null)
        {
            return Result<bool>.Failure(UserErrors.NotFound);
        }

        patient.FirstName = request.FirstName;
        patient.MiddleName = request.MiddleName;
        patient.LastName = request.LastName;
        patient.PhoneNumber = request.PhoneNumber;
        patient.DateOfBirth = request.DateOfBirth;
        patient.Gender = request.Gender;
        patient.BloodType = request.BloodType;
        patient.EmergencyContact = request.EmergencyContact;

        // 👈 استخدام دالة Update من الواجهة الخاصة بك
        _context.Update(patient);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}