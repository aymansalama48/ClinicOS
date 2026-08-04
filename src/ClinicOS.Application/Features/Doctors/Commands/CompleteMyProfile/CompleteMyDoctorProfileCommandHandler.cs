using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Application.Common.Errors.Doctors;
using ClinicOS.Application.Common.Errors.Users; // ?? ÷›‰« œÂ ⁄‘«‰ UserErrors
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Doctors;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Doctors.Commands.CompleteMyProfile;

public sealed class CompleteMyDoctorProfileCommandHandler : ICommandHandler<CompleteMyDoctorProfileCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CompleteMyDoctorProfileCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CompleteMyDoctorProfileCommand request, CancellationToken cancellationToken)
    {
        // ?? «· √ﬂœ „‰ ≈‰ «·‹ ID „ÊÃÊœ Ê„›Ì‘ ›ÌÂ „‘ﬂ·…
        if (!_currentUser.UserId.HasValue || _currentUser.UserId.Value == Guid.Empty)
        {
            return Result<Guid>.Failure(UserErrors.NotFound);
        }

        var userId = _currentUser.UserId.Value;

        // «” Œœ„‰« «·„ €Ì— «·ÃœÌœ (userId) 
        var profileExists = await 
            _context.Doctors.Where(d => d.ApplicationUserId == userId).AnyAsync(cancellationToken);

        if (profileExists) return Result<Guid>.Failure(DoctorErrors.ProfileAlreadyExists);

        var doctor = Doctor.Create(
            userId,
            request.SpecializationId,
            request.Bio,
            request.YearsOfExperience,
            request.ConsultationFee,
            request.UrgentSurchargeFee
        );

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(doctor.Id);
    }
}
