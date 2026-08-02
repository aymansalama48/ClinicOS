using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Patients;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Patients;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Patients.Commands.CreatePatient;

public sealed class CreatePatientCommandHandler : ICommandHandler<CreatePatientCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreatePatientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        // 1. التحقق إن رقم التليفون مش متكرر
        var phoneQuery = _context.Patients.Where(p => p.PhoneNumber == request.PhoneNumber);
        var isPhoneExists = await _context.AnyAsync(phoneQuery, cancellationToken);

        if (isPhoneExists)
        {
            return Result<Guid>.Failure(PatientErrors.PhoneNumberAlreadyExists);
        }

        // 2. إنشاء كيان المريض
        var patient = new Patient
        {
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            BloodType = request.BloodType,
            EmergencyContact = request.EmergencyContact,
            ApplicationUserId = request.ApplicationUserId
        };

        // 3. الحفظ في الداتا بيز عبر الـ Context Adapter
        _context.Add(patient);
        await _context.SaveChangesAsync(cancellationToken);

        // 4. إرجاع الـ ID الجديد
        return Result<Guid>.Success(patient.Id);
    }
}