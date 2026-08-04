using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Models;
using ClinicOS.Domain.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctorsLookup;

public sealed class GetDoctorsLookupQueryHandler : IQueryHandler<GetDoctorsLookupQuery, List<LookupResponse>>
{
    private readonly IReadDbContext _context;
    private readonly IUserManagementService _userService;

    public GetDoctorsLookupQueryHandler(IReadDbContext context, IUserManagementService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<Result<List<LookupResponse>>> Handle(GetDoctorsLookupQuery request, CancellationToken cancellationToken)
    {
        var doctors = await _context.Doctors.ToListAsync(cancellationToken);
        
        var userIds = doctors.Select(d => d.ApplicationUserId).Distinct().ToList();
        var users = await _userService.GetUsersByIdsAsync(userIds, cancellationToken);
        var usersDict = users.ToDictionary(u => u.Id);

        var lookups = doctors.Select(d => 
        {
            var user = usersDict.GetValueOrDefault(d.ApplicationUserId);
            return new LookupResponse(d.Id, user?.FullName ?? "Unknown");
        });

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            lookups = lookups.Where(l => l.Name.ToLower().Contains(searchTerm));
        }

        return Result<List<LookupResponse>>.Success(lookups.ToList());
    }
}
