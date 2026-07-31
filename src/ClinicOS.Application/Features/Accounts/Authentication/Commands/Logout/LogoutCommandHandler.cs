using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Features.Accounts.Authentication.Commands.Logout;

public sealed class LogoutCommandHandler(IStaffAuthService staffAuthService)
    : ICommandHandler<LogoutCommand, bool>
{
    public async Task<Result<bool>> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        return await staffAuthService.LogoutAsync(
            request.RefreshToken,
            cancellationToken);
    }
}