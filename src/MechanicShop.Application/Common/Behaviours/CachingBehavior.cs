using System.ComponentModel;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Common.Behaviors;

public class CachingBehavior<TRequest, TResponse>(
    ILogger<CachingBehavior<TRequest, TResponse>> logger,
    HybridCache hybridCache
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger = logger;
    private readonly HybridCache _hybridCache = hybridCache;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (request is not ICachedQuery cachedQuery)
        {
            return await next(cancellationToken);
        }

        _logger.LogInformation("checking cache for {EntityName}.", typeof(TResponse).Name);
        var result = await _hybridCache.GetOrCreateAsync(
            key: cachedQuery.CacheKey,
            factory: _ => new ValueTask<TResponse>(null!), // just to return null, cause the factory cannot be run in here we need to check if it's success
            options: new HybridCacheEntryOptions
            {
                Flags = HybridCacheEntryFlags.DisableUnderlyingData,
            },
            cancellationToken: cancellationToken
        );

        // if result is null cache miss
        if (result is null)
        {
            result = await next(cancellationToken);

            if (result is IResult res && res.IsSuccess)
            {
                _logger.LogInformation("storing cache for {EntityName}.", typeof(TResponse).Name);
                await _hybridCache.SetAsync(
                    cachedQuery.CacheKey,
                    result,
                    new HybridCacheEntryOptions { Expiration = cachedQuery.Expiration },
                    cancellationToken: cancellationToken
                );
            }
        }
        return result;
    }
}
