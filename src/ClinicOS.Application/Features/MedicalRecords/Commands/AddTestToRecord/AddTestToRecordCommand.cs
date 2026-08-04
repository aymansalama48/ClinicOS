using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Domain.Entities.MedicalRecords;
using ClinicOS.Domain.Common.Results;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.MedicalRecords.Commands.AddTestToRecord;

public record AddTestToRecordCommand(Guid MedicalRecordId, string TestName, string ResultDescription) : IRequest<Result<bool>>;

public class AddTestToRecordCommandHandler(IApplicationDbContext context) : IRequestHandler<AddTestToRecordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AddTestToRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await context.MedicalRecords.FindAsync(new object[] { request.MedicalRecordId }, cancellationToken);
        if (record == null)
            throw new Exception("Medical Record not found");

        var test = Test.Create(request.TestName, request.ResultDescription);
        record.AddTest(test);

        await context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
