using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ClinicOS.Application.Common.Behaviors;

public sealed class CacheInvalidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheInvalidatorCommand
{
    private readonly ICacheService _cacheService;

    private readonly ILogger<
        CacheInvalidationBehavior<TRequest, TResponse>> _logger;

    public CacheInvalidationBehavior(
        ICacheService cacheService,
        ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Execute the command first.
        var response = await next();

        // Never invalidate cache when the command failed.
        if (response is Result { IsSuccess: false })
        {
            _logger.LogDebug(
                "Command failed. Cache invalidation skipped.");

            return response;
        }

        if (request.CacheKeys.Count == 0)
        {
            return response;
        }

        foreach (var cacheKey in request.CacheKeys)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _cacheService.RemoveAsync(
                cacheKey,
                cancellationToken);

            _logger.LogDebug(
                "Invalidated cache key {CacheKey}",
                cacheKey);
        }

        return response;
    }
}