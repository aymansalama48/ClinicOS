using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Identity.Tokens;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Features.Accounts.StaffAuth.Shared;
using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Features.Accounts.Authentication.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(IRefreshTokenService refreshTokenService)
    : ICommandHandler<RefreshTokenCommand, StaffAuthResponse>
{
    public async Task<Result<StaffAuthResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        return await refreshTokenService.RefreshTokenAsync(
            request.RefreshToken,
            cancellationToken);
    }
}