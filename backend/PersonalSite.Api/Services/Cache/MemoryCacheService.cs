using Microsoft.Extensions.Caching.Memory;

namespace PersonalSite.Api.Services.Cache;

/// <summary>
/// 内存缓存服务实现
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<T?> GetAsync<T>(string key) where T : class
    {
        _memoryCache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        var options = new MemoryCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration;
        }
        else
        {
            options.SlidingExpiration = TimeSpan.FromMinutes(10);
        }

        _memoryCache.Set(key, value, options);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        return Task.FromResult(_memoryCache.TryGetValue(key, out _));
    }

    public Task<long> IncrementAsync(string key, TimeSpan? expiration = null)
    {
        var count = _memoryCache.GetOrCreate(key, entry =>
        {
            if (expiration.HasValue)
            {
                entry.AbsoluteExpirationRelativeToNow = expiration;
            }
            else
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
            }
            return 0L;
        });

        count++;
        _memoryCache.Set(key, count);
        return Task.FromResult(count);
    }

    public Task<long> GetCountAsync(string key)
    {
        if (_memoryCache.TryGetValue(key, out long count))
        {
            return Task.FromResult(count);
        }
        return Task.FromResult(0L);
    }
}

