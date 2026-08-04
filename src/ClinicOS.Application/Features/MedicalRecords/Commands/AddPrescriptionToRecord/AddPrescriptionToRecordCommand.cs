using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Domain.Entities.Prescriptions;
using ClinicOS.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.MedicalRecords.Commands.AddPrescriptionToRecord;

public record AddPrescriptionToRecordCommand(Guid MedicalRecordId, string Medications, string Notes) : IRequest<Result<bool>>;

public class AddPrescriptionToRecordCommandHandler(IApplicationDbContext context) : IRequestHandler<AddPrescriptionToRecordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AddPrescriptionToRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await context.MedicalRecords.FindAsync(new object[] { request.MedicalRecordId }, cancellationToken);
        if (record == null)
            throw new Exception("Medical Record not found");

        var prescription = Prescription.Create(request.Medications, request.Notes);
        record.AddPrescription(prescription);

        await context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
