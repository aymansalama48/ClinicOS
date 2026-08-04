using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Models;
using ClinicOS.Domain.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Application.Features.Patients.Queries.GetPatientsLookup;

public sealed class GetPatientsLookupQueryHandler : IQueryHandler<GetPatientsLookupQuery, List<LookupResponse>>
{
    private readonly IReadDbContext _context;

    public GetPatientsLookupQueryHandler(IReadDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<LookupResponse>>> Handle(GetPatientsLookupQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Patients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(p => 
                p.FirstName.ToLower().Contains(searchTerm) || 
                p.LastName.ToLower().Contains(searchTerm) ||
                (p.MiddleName != null && p.MiddleName.ToLower().Contains(searchTerm)));
        }

        var patients = await query
            .Select(p => new LookupResponse(p.Id, p.FirstName + " " + p.LastName))
            .ToListAsync(cancellationToken);

        return Result<List<LookupResponse>>.Success(patients);
    }
}
