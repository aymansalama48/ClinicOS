using ClinicOS.Application.Common.Abstractions.Identity.Providers;
using ClinicOS.Application.Common.Errors.Identity;
using ClinicOS.Domain.Common.Results;

using ClinicOS.Infrastructure.Options;
using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Server;

namespace ClinicOS.Infrastructure.Identity.Providers;

public sealed class GoogleAuthProvider : IExternalAuthProvider
{
    private readonly GoogleAuthOptions _options;
    private readonly ILogger<GoogleAuthProvider> _logger;

    public GoogleAuthProvider(IOptions<GoogleAuthOptions> options, ILogger<GoogleAuthProvider> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public string ProviderName => "Google";

    public async Task<Result<ExternalUserResult>> ValidateTokenAsync(
        string idToken,
        CancellationToken cancellationToken)   // 👈 مضافة، زي كل مكان تاني في المشروع
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings();

            if (!string.IsNullOrWhiteSpace(_options.ClientId))
                settings.Audience = new[] { _options.ClientId };

            // ملحوظة: المكتبة دي مالهاش Overload بياخد CancellationToken (قيد خارجي زي UserManager بالظبط)
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            if (string.IsNullOrWhiteSpace(payload.Email))
                return Result<ExternalUserResult>.Failure(ExternalAuthErrors.EmailMissing);

            if (!payload.EmailVerified)   // 👈 مهم أمنيًا — إيميل جوجل نفسه لازم يكون Verified
                return Result<ExternalUserResult>.Failure(ExternalAuthErrors.EmailNotVerified);

            return Result<ExternalUserResult>.Success(new ExternalUserResult(
                ProviderUserId: payload.Subject,
                Email: payload.Email.ToLowerInvariant(),
                FullName: payload.Name ?? payload.Email,
                AvatarUrl: payload.Picture));
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning(ex, "توكن Google غير صالح");
            return Result<ExternalUserResult>.Failure(ExternalAuthErrors.InvalidToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ غير متوقع أثناء التحقق من توكن Google");
            return Result<ExternalUserResult>.Failure(ExternalAuthErrors.InvalidToken);
        }
    }
}