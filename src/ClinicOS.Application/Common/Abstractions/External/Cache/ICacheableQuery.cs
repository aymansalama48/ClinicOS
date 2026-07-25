using ClinicOS.Application.Common.Abstractions.Messaging;

namespace ClinicOS.Application.Common.Abstractions.External.Cache;

public interface ICacheableQuery<TResponse> : IQuery<TResponse>
{
    string CacheKey { get; }

    TimeSpan? SlidingExpiration =>
        TimeSpan.FromMinutes(5);

    TimeSpan? AbsoluteExpiration =>
        TimeSpan.FromMinutes(30);
}