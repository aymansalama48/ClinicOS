using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Messaging;

namespace ClinicOS.Application.Features.Specializations.Commands.CreateSpecialization;

public sealed record CreateSpecializationCommand(
    string Name,
    string? Description
) : ICommand<Guid>, ICacheInvalidatorCommand
{
    // عند إنشاء تخصص جديد، نحتاج لمسح كاش القوائم المرتبطة بالتخصصات تلقائياً
    public IReadOnlyCollection<string> CacheKeys => new[] { "specializations-all" };
}