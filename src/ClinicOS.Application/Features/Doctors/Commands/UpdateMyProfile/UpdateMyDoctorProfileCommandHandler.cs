using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Doctors;
using ClinicOS.Domain.Common.Results;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Doctors.Commands.UpdateMyProfile;

public sealed class UpdateMyDoctorProfileCommandHandler : ICommandHandler<UpdateMyDoctorProfileCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public UpdateMyDoctorProfileCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(UpdateMyDoctorProfileCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _context.FirstOrDefaultAsync(
            _context.Doctors.Where(d => d.ApplicationUserId == _currentUser.UserId), cancellationToken);

        if (doctor is null) return Result<Guid>.Failure(DoctorErrors.ProfileNotFound);

        doctor.UpdateProfile(
            request.SpecializationId,
            request.Bio,
            request.YearsOfExperience,
            request.ConsultationFee,
            request.UrgentSurchargeFee
        );

        _context.Update(doctor);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(doctor.Id);
    }
}