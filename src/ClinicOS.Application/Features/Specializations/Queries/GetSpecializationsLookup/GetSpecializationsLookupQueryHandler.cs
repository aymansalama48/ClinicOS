using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Models;
using ClinicOS.Domain.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Application.Features.Specializations.Queries.GetSpecializationsLookup;

public sealed class GetSpecializationsLookupQueryHandler : IQueryHandler<GetSpecializationsLookupQuery, List<LookupResponse>>
{
    private readonly IReadDbContext _context;

    public GetSpecializationsLookupQueryHandler(IReadDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<LookupResponse>>> Handle(GetSpecializationsLookupQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Specializations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(searchTerm));
        }

        var specializations = await query
            .Select(s => new LookupResponse(s.Id, s.Name))
            .ToListAsync(cancellationToken);

        return Result<List<LookupResponse>>.Success(specializations);
    }
}
