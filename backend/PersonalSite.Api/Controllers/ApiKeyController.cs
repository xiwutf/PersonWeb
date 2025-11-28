using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using System.Security.Cryptography;
using System.Text;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// API Key 管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ApiKeyController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ApiKeyController> _logger;

    public ApiKeyController(AppDbContext context, ILogger<ApiKeyController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 注册API用户
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<object>>> Register([FromBody] ApiUserRegisterRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(ApiResponse.Error("Email and Password are required", 400));
            }

            // 检查邮箱是否已存在
            var existingUser = await _context.ApiUsers
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (existingUser != null)
            {
                return BadRequest(ApiResponse.Error("邮箱已被注册", 400));
            }

            // 创建用户
            var user = new ApiUser
            {
                Email = request.Email,
                PasswordHash = HashPassword(request.Password), // 实际应该使用更安全的哈希方法
                Name = request.Name,
                PlanType = "free",
                Status = "active"
            };

            _context.ApiUsers.Add(user);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new
            {
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name,
                PlanType = user.PlanType,
                Message = "注册成功"
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "注册API用户失败");
            return StatusCode(500, ApiResponse.Error($"注册失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 生成API Key
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<ApiResponse<object>>> GenerateApiKey([FromBody] GenerateApiKeyRequest request)
    {
        try
        {
            if (request.UserId <= 0)
            {
                return BadRequest(ApiResponse.Error("UserId is required", 400));
            }

            // 验证用户
            var user = await _context.ApiUsers.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound(ApiResponse.Error("用户不存在", 404));
            }

            // 生成API Key和Secret
            var apiKey = GenerateSecureKey();
            var apiSecret = GenerateSecureKey();
            var hashedSecret = HashPassword(apiSecret);

            var keyRecord = new ApiKey
            {
                UserId = request.UserId,
                ApiKeyValue = apiKey,
                ApiSecret = hashedSecret,
                Name = request.Name ?? "默认Key",
                RateLimit = request.RateLimit ?? 100,
                DailyLimit = request.DailyLimit ?? 10000,
                ExpiresAt = request.ExpiresAt,
                IsActive = true
            };

            _context.ApiKeys.Add(keyRecord);
            await _context.SaveChangesAsync();

            // 返回原始Secret（只返回一次）
            return Ok(ApiResponse.Success(new
            {
                KeyId = keyRecord.Id,
                ApiKey = apiKey,
                ApiSecret = apiSecret, // 仅此一次返回
                Name = keyRecord.Name,
                RateLimit = keyRecord.RateLimit,
                DailyLimit = keyRecord.DailyLimit,
                ExpiresAt = keyRecord.ExpiresAt,
                Message = "请妥善保管API Secret，它只会显示一次"
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成API Key失败");
            return StatusCode(500, ApiResponse.Error($"生成失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取用户的API Keys
    /// </summary>
    [HttpPost("list")]
    public async Task<ActionResult<ApiResponse<object>>> GetApiKeys([FromBody] ApiUserIdRequest request)
    {
        try
        {
            var keys = await _context.ApiKeys
                .Where(k => k.UserId == request.UserId && k.IsActive)
                .Select(k => new
                {
                    k.Id,
                    k.Name,
                    ApiKey = k.ApiKeyValue.Substring(0, 8) + "****", // 只显示前8位
                    k.RateLimit,
                    k.DailyLimit,
                    k.ExpiresAt,
                    k.LastUsedAt,
                    k.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse.Success(keys));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取API Keys失败");
            return StatusCode(500, ApiResponse.Error($"获取失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取API调用统计
    /// </summary>
    [HttpPost("stats")]
    public async Task<ActionResult<ApiResponse<object>>> GetApiStats([FromBody] ApiStatsRequest request)
    {
        try
        {
            var query = _context.ApiCalls.Where(c => c.ApiKeyId == request.ApiKeyId);

            if (request.StartDate.HasValue)
            {
                query = query.Where(c => c.CalledAt >= request.StartDate.Value);
            }
            if (request.EndDate.HasValue)
            {
                query = query.Where(c => c.CalledAt <= request.EndDate.Value);
            }

            var totalCalls = await query.CountAsync();
            var successCalls = await query.CountAsync(c => c.StatusCode >= 200 && c.StatusCode < 300);
            var failedCalls = totalCalls - successCalls;
            var avgResponseTime = await query
                .Where(c => c.ResponseTime.HasValue)
                .Select(c => c.ResponseTime!.Value)
                .DefaultIfEmpty(0)
                .AverageAsync();

            // 按日期统计
            var dailyStats = await query
                .GroupBy(c => c.CalledAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count(),
                    SuccessCount = g.Count(c => c.StatusCode >= 200 && c.StatusCode < 300),
                    AvgResponseTime = g.Where(c => c.ResponseTime.HasValue)
                        .Select(c => c.ResponseTime!.Value)
                        .DefaultIfEmpty(0)
                        .Average()
                })
                .OrderBy(s => s.Date)
                .ToListAsync();

            return Ok(ApiResponse.Success(new
            {
                TotalCalls = totalCalls,
                SuccessCalls = successCalls,
                FailedCalls = failedCalls,
                AvgResponseTime = Math.Round(avgResponseTime, 2),
                DailyStats = dailyStats
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取统计失败");
            return StatusCode(500, ApiResponse.Error($"获取统计失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 删除API Key
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteApiKey(long id, [FromBody] ApiUserIdRequest request)
    {
        try
        {
            var apiKey = await _context.ApiKeys
                .FirstOrDefaultAsync(k => k.Id == id && k.UserId == request.UserId);

            if (apiKey == null)
            {
                return NotFound(ApiResponse.Error("API Key不存在", 404));
            }

            apiKey.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(null, "API Key已删除"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除API Key失败");
            return StatusCode(500, ApiResponse.Error($"删除失败: {ex.Message}", 500));
        }
    }

    // 辅助方法：生成安全密钥
    private string GenerateSecureKey()
    {
        var bytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    // 辅助方法：哈希密码（简化版，实际应使用BCrypt等）
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
    }
}

// 请求模型
public class ApiUserRegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Name { get; set; }
}

public class GenerateApiKeyRequest
{
    public long UserId { get; set; }
    public string? Name { get; set; }
    public int? RateLimit { get; set; }
    public int? DailyLimit { get; set; }
    public DateTime? ExpiresAt { get; set; }
}


public class ApiStatsRequest
{
    public long ApiKeyId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

