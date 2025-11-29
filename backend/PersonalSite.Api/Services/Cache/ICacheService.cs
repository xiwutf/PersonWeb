namespace PersonalSite.Api.Services.Cache;

/// <summary>
/// 缓存服务接口（支持内存缓存和Redis）
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// 获取缓存值
    /// </summary>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// 设置缓存值
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// 删除缓存
    /// </summary>
    Task RemoveAsync(string key);

    /// <summary>
    /// 检查键是否存在
    /// </summary>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// 增加计数（用于限流）
    /// </summary>
    Task<long> IncrementAsync(string key, TimeSpan? expiration = null);

    /// <summary>
    /// 获取计数
    /// </summary>
    Task<long> GetCountAsync(string key);
}

