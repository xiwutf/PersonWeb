using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalSite.Api.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PersonalSite.Api.Middleware;

/// <summary>
/// API Key 验证中间件
/// 验证请求中的 API Key 并设置用户上下文
/// </summary>
public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyAuthMiddleware> _logger;

    public ApiKeyAuthMiddleware(RequestDelegate next, ILogger<ApiKeyAuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 跳过不需要 API Key 验证的端点
        if (ShouldSkipApiKeyAuth(context))
        {
            await _next(context);
            return;
        }

        // 获取 API Key
        var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault() ??
                     context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        if (string.IsNullOrEmpty(apiKey))
        {
            // 如果没有 API Key，继续执行（可能是其他认证方式）
            await _next(context);
            return;
        }

        // 验证 API Key
        var validationResult = await ValidateApiKeyAsync(context, apiKey);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("API Key 验证失败: {Reason}", validationResult.ErrorMessage);

            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = validationResult.ErrorMessage ?? "无效的 API Key",
                code = 401
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            return;
        }

        // 将用户信息存储到 HttpContext.Items 中，供后续使用
        if (validationResult.UserId != null)
        {
            context.Items["ApiUserId"] = validationResult.UserId;
            context.Items["ApiKeyId"] = validationResult.ApiKeyId;
        }

        await _next(context);
    }

    /// <summary>
    /// 检查是否应该跳过 API Key 验证
    /// </summary>
    private bool ShouldSkipApiKeyAuth(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";

        // 跳过健康检查、Swagger、公开 API 等
        if (path.StartsWith("/api/health") ||
            path.StartsWith("/swagger") ||
            path.StartsWith("/api/docs") ||
            path.StartsWith("/api/auth") ||
            path.StartsWith("/api/toolbox/marketplace") ||
            path.StartsWith("/api/toolbox/categories"))
        {
            return true;
        }

        // 只对需要 API Key 的端点进行验证
        // 例如：/api/tool-api/*, /api/api-key/* 等
        if (path.StartsWith("/api/tool-api") ||
            path.StartsWith("/api/api-key") ||
            path.StartsWith("/api/toolbox/purchase") ||
            path.StartsWith("/api/toolbox/my-tools"))
        {
            return false; // 需要验证
        }

        // 其他端点可选验证
        return true;
    }

    /// <summary>
    /// 验证 API Key
    /// </summary>
    private async Task<ApiKeyValidationResult> ValidateApiKeyAsync(HttpContext context, string apiKey)
    {
        try
        {
            // 获取数据库上下文
            var dbContext = context.RequestServices.GetRequiredService<AppDbContext>();

            // 哈希 API Key（用于 ToolApiKey 验证）
            var hashedKey = HashApiKey(apiKey);

            // 查找 API Key（支持两种类型：ToolApiKey 和 ApiKey）
            // 先查找 ToolApiKey（存储的是哈希值）
            var toolApiKey = await dbContext.ToolApiKeys
                .Include(k => k.Tool)
                .FirstOrDefaultAsync(k =>
                    k.ApiKey == hashedKey &&
                    k.IsActive &&
                    (k.ExpiresAt == null || k.ExpiresAt > DateTime.Now));

            if (toolApiKey != null)
            {
                return new ApiKeyValidationResult
                {
                    IsValid = true,
                    UserId = toolApiKey.UserId,
                    ApiKeyId = toolApiKey.Id,
                    KeyType = "ToolApiKey"
                };
            }

            // 查找通用 ApiKey（存储的是原始值，需要匹配 ApiKeyValue）
            var generalApiKey = await dbContext.ApiKeys
                .Include(k => k.User)
                .FirstOrDefaultAsync(k =>
                    k.ApiKeyValue == apiKey &&
                    k.IsActive &&
                    (k.ExpiresAt == null || k.ExpiresAt > DateTime.Now));

            if (generalApiKey != null)
            {
                return new ApiKeyValidationResult
                {
                    IsValid = true,
                    UserId = generalApiKey.UserId.ToString(),
                    ApiKeyId = generalApiKey.Id,
                    KeyType = "ApiKey"
                };
            }

            return new ApiKeyValidationResult
            {
                IsValid = false,
                ErrorMessage = "无效的 API Key 或已过期"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证 API Key 时发生错误");
            return new ApiKeyValidationResult
            {
                IsValid = false,
                ErrorMessage = "验证 API Key 时发生错误"
            };
        }
    }

    /// <summary>
    /// 哈希 API Key
    /// </summary>
    private string HashApiKey(string apiKey)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
        return Convert.ToBase64String(hash);
    }
}

/// <summary>
/// API Key 验证结果
/// </summary>
internal class ApiKeyValidationResult
{
    public bool IsValid { get; set; }
    public string? UserId { get; set; }
    public long? ApiKeyId { get; set; }
    public string? KeyType { get; set; }
    public string? ErrorMessage { get; set; }
}

