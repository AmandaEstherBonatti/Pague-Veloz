using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PagueVeloz.Application.Interfaces;

namespace PagueVeloz.Infrastructure.Caching;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryCacheService> _logger;

    public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        _cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        var options = new MemoryCacheEntryOptions();

        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration;
        }
        else
        {
            options.SlidingExpiration = TimeSpan.FromMinutes(5);
        }

        _cache.Set(key, value, options);
        _logger.LogDebug("Cache set: {Key}", key);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);
        _logger.LogDebug("Cache removed: {Key}", key);
        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // Para MemoryCache, não há suporte nativo para remover por padrão
        // Em produção, use Redis ou outro cache distribuído
        _logger.LogWarning("RemoveByPattern não é suportado em MemoryCache. Use Redis para cache distribuído.");
        return Task.CompletedTask;
    }
}

