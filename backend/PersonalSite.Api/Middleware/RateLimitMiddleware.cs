using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PersonalSite.Api.Services.Cache;
using System.Net;
using System.Text.Json;

namespace PersonalSite.Api.Middleware;

/// <summary>
/// API 限流中间件
/// 支持基于 API Key 的限流和基于 IP 的限流
/// </summary>
public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICacheService _cacheService;
    private readonly ILogger<RateLimitMiddleware> _logger;
    private readonly RateLimitOptions _options;

    public RateLimitMiddleware(
        RequestDelegate next,
        ICacheService cacheService,
        ILogger<RateLimitMiddleware> logger,
        IOptions<RateLimitOptions> options)
    {
        _next = next;
        _cacheService = cacheService;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 跳过健康检查等不需要限流的端点
        if (ShouldSkipRateLimit(context))
        {
            await _next(context);
            return;
        }

        // 获取限流标识（API Key 或 IP 地址）
        string rateLimitKey = GetRateLimitKey(context);

        if (string.IsNullOrEmpty(rateLimitKey))
        {
            await _next(context);
            return;
        }

        // 检查限流
        var rateLimitResult = await CheckRateLimitAsync(rateLimitKey, context);

        if (!rateLimitResult.IsAllowed)
        {
            _logger.LogWarning(
                "请求被限流: {Key}, 路径: {Path}, 限制: {Limit}/{Period}秒",
                rateLimitKey, context.Request.Path, rateLimitResult.Limit, _options.WindowSeconds);

            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = $"请求过于频繁，请稍后再试。限制: {rateLimitResult.Limit} 次/{_options.WindowSeconds}秒",
                code = 429,
                data = new
                {
                    limit = rateLimitResult.Limit,
                    remaining = rateLimitResult.Remaining,
                    resetAt = rateLimitResult.ResetAt
                }
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            return;
        }

        // 添加限流响应头
        context.Response.Headers["X-RateLimit-Limit"] = rateLimitResult.Limit.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = rateLimitResult.Remaining.ToString();
        context.Response.Headers["X-RateLimit-Reset"] = rateLimitResult.ResetAt.ToString("O");

        await _next(context);
    }

    /// <summary>
    /// 检查是否应该跳过限流
    /// </summary>
    private bool ShouldSkipRateLimit(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";

        // 跳过健康检查、Swagger 等
        if (path.StartsWith("/api/health") ||
            path.StartsWith("/swagger") ||
            path.StartsWith("/api/docs"))
        {
            return true;
        }

        // 跳过登录接口（避免登录时被限流）
        // 注意：登录接口本身应该有其他安全措施（如验证码、账户锁定等）
        if (path == "/api/auth/login" || path == "/api/auth/refresh")
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 获取限流标识键
    /// 优先级：API Key > 用户 ID (从 JWT) > IP 地址
    /// </summary>
    private string GetRateLimitKey(HttpContext context)
    {
        // 优先使用 API Key（X-API-Key header）
        var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();
        if (!string.IsNullOrEmpty(apiKey))
        {
            return $"apikey:{apiKey}";
        }

        // 获取 IP 地址（在方法开始处获取，避免重复声明）
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        // 检查 Authorization header
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Replace("Bearer ", "").Trim();
            
            // 判断是否是 JWT token（JWT 格式：header.payload.signature，用 . 分隔）
            if (IsJwtToken(token))
            {
                // 对于 JWT token，使用 IP 地址进行限流
                // 原因：
                // 1. JWT token 可能每次不同（如果实现了 token 刷新）
                // 2. 解析 JWT token 有性能开销
                // 3. 使用 IP 地址可以更好地防止滥用
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    return $"ip:{ipAddress}";
                }
            }
            else
            {
                // 非 JWT token，可能是 API Key，使用完整 token 作为标识
                return $"apikey:{token}";
            }
        }

        // 使用 IP 地址
        if (!string.IsNullOrEmpty(ipAddress))
        {
            return $"ip:{ipAddress}";
        }

        return string.Empty;
    }

    /// <summary>
    /// 判断是否是 JWT token
    /// JWT 格式：header.payload.signature（三部分用 . 分隔）
    /// </summary>
    private bool IsJwtToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        var parts = token.Split('.');
        // JWT 应该有 3 部分
        return parts.Length == 3;
    }

    /// <summary>
    /// 检查限流
    /// </summary>
    private async Task<RateLimitResult> CheckRateLimitAsync(string key, HttpContext context)
    {
        var cacheKey = $"ratelimit:{key}";
        var windowStart = DateTime.UtcNow.AddSeconds(-_options.WindowSeconds);
        var expiration = TimeSpan.FromSeconds(_options.WindowSeconds * 2);

        // 使用缓存服务获取限流信息
        var info = await _cacheService.GetAsync<RateLimitInfo>(cacheKey);

        if (info != null)
        {
            // 如果窗口已过期，重置
            if (info.WindowStart < windowStart)
            {
                info = new RateLimitInfo
                {
                    Count = 0,
                    WindowStart = DateTime.UtcNow
                };
            }

            // 检查是否超过限制
            if (info.Count >= _options.DefaultLimit)
            {
                return new RateLimitResult
                {
                    IsAllowed = false,
                    Limit = _options.DefaultLimit,
                    Remaining = 0,
                    ResetAt = info.WindowStart.AddSeconds(_options.WindowSeconds)
                };
            }

            // 增加计数
            info.Count++;

            // 更新缓存
            await _cacheService.SetAsync(cacheKey, info, expiration);

            return new RateLimitResult
            {
                IsAllowed = true,
                Limit = _options.DefaultLimit,
                Remaining = _options.DefaultLimit - info.Count,
                ResetAt = info.WindowStart.AddSeconds(_options.WindowSeconds)
            };
        }

        // 创建新的限流信息
        info = new RateLimitInfo
        {
            Count = 1,
            WindowStart = DateTime.UtcNow
        };

        await _cacheService.SetAsync(cacheKey, info, expiration);

        return new RateLimitResult
        {
            IsAllowed = true,
            Limit = _options.DefaultLimit,
            Remaining = _options.DefaultLimit - 1,
            ResetAt = DateTime.UtcNow.AddSeconds(_options.WindowSeconds)
        };
    }
}

/// <summary>
/// 限流配置选项
/// </summary>
public class RateLimitOptions
{
    /// <summary>
    /// 默认限流数量
    /// </summary>
    public int DefaultLimit { get; set; } = 100;

    /// <summary>
    /// 时间窗口（秒）
    /// </summary>
    public int WindowSeconds { get; set; } = 60;
}

/// <summary>
/// 限流信息
/// </summary>
public class RateLimitInfo
{
    public int Count { get; set; }
    public DateTime WindowStart { get; set; }
}

/// <summary>
/// 限流检查结果
/// </summary>
internal class RateLimitResult
{
    public bool IsAllowed { get; set; }
    public int Limit { get; set; }
    public int Remaining { get; set; }
    public DateTime ResetAt { get; set; }
}

