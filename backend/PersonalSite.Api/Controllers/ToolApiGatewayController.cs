using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 工具API网关控制器（供外部开发者调用）
/// </summary>
[ApiController]
[Route("api/tool-api")]
public class ToolApiGatewayController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ToolApiGatewayController> _logger;

    public ToolApiGatewayController(AppDbContext context, ILogger<ToolApiGatewayController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 调用工具API（通用接口）
    /// </summary>
    [HttpPost("{toolId}/execute")]
    public async Task<ActionResult<ApiResponse<object>>> ExecuteTool(
        long toolId,
        [FromHeader(Name = "X-API-Key")] string? apiKey,
        [FromBody] Dictionary<string, object>? requestData
    )
    {
        try
        {
            // 验证API密钥
            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized(ApiResponse.Error("API密钥不能为空", 401));
            }

            var hashedKey = HashApiKey(apiKey);
            var keyRecord = await _context.ToolApiKeys
                .Include(k => k.Tool)
                .FirstOrDefaultAsync(k => 
                    k.ApiKey == hashedKey && 
                    k.ToolId == toolId && 
                    k.IsActive &&
                    (k.ExpiresAt == null || k.ExpiresAt > DateTime.Now));

            if (keyRecord == null)
            {
                return Unauthorized(ApiResponse.Error("无效的API密钥", 401));
            }

            // 检查速率限制
            var hourAgo = DateTime.Now.AddHours(-1);
            var recentUsage = await _context.ToolUsages
                .CountAsync(u => 
                    u.ToolId == toolId && 
                    u.UserId == keyRecord.UserId && 
                    u.UsageType == "api" &&
                    u.CreatedAt >= hourAgo);

            if (recentUsage >= keyRecord.RateLimit)
            {
                return StatusCode(429, ApiResponse.Error($"速率限制：每小时最多 {keyRecord.RateLimit} 次请求", 429));
            }

            var tool = keyRecord.Tool;
            if (tool == null)
            {
                return NotFound(ApiResponse.Error("工具不存在", 404));
            }

            // 记录使用
            var startTime = DateTime.Now;
            var usage = new Models.ToolUsage
            {
                ToolId = toolId,
                UserId = keyRecord.UserId,
                UsageType = "api",
                RequestData = requestData != null ? JsonSerializer.Serialize(requestData) : null,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString()
            };

            try
            {
                // 这里应该调用实际的工具执行逻辑
                // 目前返回模拟结果
                var result = await ExecuteToolLogic(tool, requestData);
                var executionTime = (int)(DateTime.Now - startTime).TotalMilliseconds;

                usage.ResponseData = JsonSerializer.Serialize(result);
                usage.ExecutionTime = executionTime;
                usage.Status = "success";

                // 更新API密钥使用统计
                keyRecord.UsageCount++;
                keyRecord.LastUsedAt = DateTime.Now;

                // 更新工具使用次数
                tool.UseCount++;

                _context.ToolUsages.Add(usage);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse.Success(result));
            }
            catch (Exception ex)
            {
                var executionTime = (int)(DateTime.Now - startTime).TotalMilliseconds;
                usage.ExecutionTime = executionTime;
                usage.Status = "error";
                usage.ErrorMessage = ex.Message;

                _context.ToolUsages.Add(usage);
                await _context.SaveChangesAsync();

                _logger.LogError(ex, "工具执行失败");
                return StatusCode(500, ApiResponse.Error($"工具执行失败: {ex.Message}", 500));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API调用失败");
            return StatusCode(500, ApiResponse.Error($"API调用失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取工具API文档
    /// </summary>
    [HttpGet("{toolId}/docs")]
    public async Task<ActionResult<ApiResponse<object>>> GetApiDocs(long toolId)
    {
        try
        {
            var tool = await _context.Tools
                .FirstOrDefaultAsync(t => t.Id == toolId && t.Status == "published");

            if (tool == null)
            {
                return NotFound(ApiResponse.Error("工具不存在", 404));
            }

            var docs = new
            {
                ToolId = tool.Id,
                ToolName = tool.Name,
                ApiEndpoint = tool.ApiEndpoint ?? $"/api/tool-api/{toolId}/execute",
                Documentation = tool.ApiDocumentation != null 
                    ? JsonSerializer.Deserialize<object>(tool.ApiDocumentation) 
                    : null,
                Requirements = tool.Requirements,
                Version = tool.Version
            };

            return Ok(ApiResponse.Success(docs));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取API文档失败");
            return StatusCode(500, ApiResponse.Error($"获取API文档失败: {ex.Message}", 500));
        }
    }

    // 辅助方法：执行工具逻辑（这里应该根据工具类型调用不同的执行器）
    private async Task<object> ExecuteToolLogic(Models.Tool tool, Dictionary<string, object>? requestData)
    {
        // 这里应该根据工具的实际逻辑来执行
        // 可以集成到 ai-service 或其他服务
        // 目前返回模拟结果
        
        await Task.Delay(100); // 模拟处理时间

        return new
        {
            Success = true,
            Message = $"工具 {tool.Name} 执行成功",
            Data = requestData,
            Timestamp = DateTime.Now
        };
    }

    // 辅助方法：哈希API密钥
    private string HashApiKey(string apiKey)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
        return Convert.ToBase64String(hash);
    }
}

