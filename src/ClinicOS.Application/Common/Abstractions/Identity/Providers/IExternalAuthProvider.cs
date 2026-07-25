using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Common.Abstractions.Identity.Providers;

public record ExternalUserResult(
    string ProviderUserId,
    string Email,
    string FullName,
    string? AvatarUrl);

public interface IExternalAuthProvider
{
    string ProviderName { get; }

    Task<Result<ExternalUserResult>> ValidateTokenAsync(
        string idToken,
        CancellationToken cancellationToken);
}