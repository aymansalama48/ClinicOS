namespace ClinicOS.Api.Extensions;

using System.Threading.RateLimiting;
using ClinicOS.Api.Middlewares;

public static class RateLimitingExtensions
{
    public const string LoginPolicy = "login-policy";
    public const string OtpVerifyPolicy = "otp-verify-policy";
    public const string OtpResendPolicy = "otp-resend-policy";
    public const string PasswordResetRequestPolicy = "password-reset-request-policy";
    public const string RefreshPolicy = "refresh-policy";
    public const string AppointmentBookingPolicy = "appointment-booking-policy";

    public static IServiceCollection AddApplicationRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // 5 attempts per 15 minutes per IP + email
            options.AddPolicy(LoginPolicy, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    BuildKey(context, "email"),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(15),
                        QueueLimit = 0
                    }));

            // 3 attempts per 10 minutes
            options.AddPolicy(OtpVerifyPolicy, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    BuildKey(context, "email"),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 3,
                        Window = TimeSpan.FromMinutes(10),
                        QueueLimit = 0
                    }));

            // 2 attempts per 10 minutes
            options.AddPolicy(OtpResendPolicy, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    BuildKey(context, "email"),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 2,
                        Window = TimeSpan.FromMinutes(10),
                        QueueLimit = 0
                    }));

            // 2 attempts per 10 minutes
            options.AddPolicy(PasswordResetRequestPolicy, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    BuildKey(context, "email"),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 2,
                        Window = TimeSpan.FromMinutes(10),
                        QueueLimit = 0
                    }));

            // 20 attempts per minute
            options.AddPolicy(RefreshPolicy, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    BuildKey(context, null),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));

            // 10 bookings per minute per IP
            options.AddPolicy(AppointmentBookingPolicy, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    BuildKey(context, null),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                TimeSpan? retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry)
                    ? retry
                    : null;

                context.HttpContext.Response.Headers["Retry-After"] =
                    retryAfter?.TotalSeconds.ToString("0") ?? "60";

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    type = "https://httpstatuses.com/429",
                    title = "Too Many Requests",
                    status = StatusCodes.Status429TooManyRequests,
                    detail = "Too many requests. Please retry later."
                }, cancellationToken);
            };
        });

        return services;
    }

    private static string BuildKey(HttpContext context, string? identityField)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (identityField is null)
            return ip;

        var identity = context.Items.TryGetValue(
                RateLimitIdentityMiddleware.ItemKey + identityField,
                out var fromBody) && fromBody is string bodyIdentity
            ? bodyIdentity
            : context.Request.Query[identityField].ToString();

        if (string.IsNullOrWhiteSpace(identity))
            return ip;

        var normalized = identity.Trim().ToLowerInvariant();

        return normalized.Length > 128 ? ip : $"{ip}|{normalized}";
    }
}
