using System.Collections.Concurrent;
using ClinicOS.Application.Common.Abstractions.External.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace ClinicOS.Infrastructure.Caching;

public sealed class MemoryCacheService : ICacheService
{
    private static readonly TimeSpan DefaultSlidingExpiration =
        TimeSpan.FromMinutes(5);

    private static readonly TimeSpan DefaultAbsoluteExpiration =
        TimeSpan.FromMinutes(30);

    private readonly IMemoryCache _memoryCache;

    private readonly ConcurrentDictionary<string, SemaphoreSlim> _keyLocks = new();

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        cancellationToken.ThrowIfCancellationRequested();

        _memoryCache.TryGetValue(key, out T? value);

        return Task.FromResult(value);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? slidingExpiration = null,
        TimeSpan? absoluteExpiration = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        cancellationToken.ThrowIfCancellationRequested();

        var options = CreateCacheOptions(
            slidingExpiration,
            absoluteExpiration);

        _memoryCache.Set(key, value, options);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        cancellationToken.ThrowIfCancellationRequested();

        _memoryCache.Remove(key);

        return Task.CompletedTask;
    }

    public async Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        Func<T, bool>? shouldCache = null,
        TimeSpan? slidingExpiration = null,
        TimeSpan? absoluteExpiration = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(factory);

        cancellationToken.ThrowIfCancellationRequested();

        // 1. Fast path: cache hit
        if (_memoryCache.TryGetValue(key, out T? cachedValue))
        {
            return cachedValue;
        }

        // 2. Get/create a lock for this specific key
        var semaphore = _keyLocks.GetOrAdd(
            key,
            static _ => new SemaphoreSlim(1, 1));

        await semaphore.WaitAsync(cancellationToken);

        try
        {
            // 3. Double-check after acquiring the lock
            if (_memoryCache.TryGetValue(key, out cachedValue))
            {
                return cachedValue;
            }

            // 4. Cache miss → execute factory
            var value = await factory(cancellationToken);

            // 5. Decide whether this value should be cached
            var cacheValue = value is not null &&
                             (shouldCache?.Invoke(value) ?? true);

            if (cacheValue)
            {
                var options = CreateCacheOptions(
                    slidingExpiration,
                    absoluteExpiration);

                _memoryCache.Set(key, value, options);
            }

            return value;
        }
        finally
        {
            semaphore.Release();
        }
    }

    private static MemoryCacheEntryOptions CreateCacheOptions(
     TimeSpan? slidingExpiration,
     TimeSpan? absoluteExpiration)
    {
        return new MemoryCacheEntryOptions
        {
            Size = 1, // 👈 إجباري لحساب حجم كل عنصر من الـ 1024 المسموح بها

            SlidingExpiration =
                slidingExpiration ?? DefaultSlidingExpiration,

            AbsoluteExpirationRelativeToNow =
                absoluteExpiration ?? DefaultAbsoluteExpiration
        };
    }
}