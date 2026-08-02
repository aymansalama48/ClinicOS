using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Receptionists;
using ClinicOS.Domain.Common.Results;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Receptionists.Queries.GetReceptionistById;

public sealed class GetReceptionistByIdQueryHandler : IQueryHandler<GetReceptionistByIdQuery, ReceptionistDetailsResponse>
{
    private readonly IApplicationDbContext _context;

    public GetReceptionistByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ReceptionistDetailsResponse>> Handle(GetReceptionistByIdQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Receptionists.Where(r => r.Id == request.Id);
        var noTrackingQuery = _context.AsNoTracking(query);

        var receptionist = await _context.FirstOrDefaultAsync(
            noTrackingQuery.Select(r => new ReceptionistDetailsResponse(
                r.Id,
                r.SpecializationId,
                r.ApplicationUserId,
                r.IsActive
            )),
            cancellationToken);

        if (receptionist is null)
        {
            return Result<ReceptionistDetailsResponse>.Failure(ReceptionistErrors.NotFound);
        }

        return Result<ReceptionistDetailsResponse>.Success(receptionist);
    }
}