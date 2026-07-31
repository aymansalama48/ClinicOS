using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Entities.Specializations;
using ClinicOS.Application.Common.Abstractions.Messaging;

namespace ClinicOS.Application.Features.Specializations.Commands.CreateSpecialization;

public sealed class CreateSpecializationCommandHandler
    : ICommandHandler<CreateSpecializationCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateSpecializationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(
        CreateSpecializationCommand request,
        CancellationToken cancellationToken)
    {
        // التحقق من عدم تكرار التخصص (بشكل أساسي في الـ Business Logic)
        // (يمكن إضافة فحص قاعدة البيانات هنا إذا رغبت)

        var specialization = new Specialization(request.Name, request.Description);

        _context.Add(specialization);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(specialization.Id);
    }
}