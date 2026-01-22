using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Text.Json;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ErrorLogController : ControllerBase
{
    private readonly AppDbContext _context;

    public ErrorLogController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 记录错误日志
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> LogError([FromBody] ErrorLogRequest request)
    {
        try
        {
            var errorLog = new ErrorLog
            {
                ErrorType = request.ErrorType ?? "Unknown",
                ErrorMessage = request.ErrorMessage ?? "Unknown error",
                ErrorStack = request.ErrorStack,
                ErrorUrl = request.ErrorUrl,
                UserIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString(),
                VisitorId = request.VisitorId,
                Metadata = request.Metadata != null ? JsonSerializer.Serialize(request.Metadata) : null,
                Status = 0,
                CreatedAt = DateTime.Now
            };

            _context.ErrorLogs.Add(errorLog);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new { Id = errorLog.Id }, "错误已记录"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"记录错误失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取错误日志列表
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetErrorLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? errorType = null,
        [FromQuery] sbyte? status = null)
    {
        var query = _context.ErrorLogs.AsQueryable();

        if (!string.IsNullOrEmpty(errorType))
        {
            query = query.Where(e => e.ErrorType == errorType);
        }

        if (status.HasValue)
        {
            query = query.Where(e => e.Status == status.Value);
        }

        var total = await query.CountAsync();
        var logs = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new
            {
                e.Id,
                e.ErrorType,
                e.ErrorMessage,
                e.ErrorUrl,
                e.UserIp,
                e.VisitorId,
                e.Status,
                e.CreatedAt,
                e.ResolvedAt
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(new { Total = total, List = logs }));
    }

    /// <summary>
    /// 获取错误日志详情
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ErrorLog>>> GetErrorLog(long id)
    {
        var log = await _context.ErrorLogs.FindAsync(id);
        if (log == null)
        {
            return Ok(ApiResponse<ErrorLog>.Error("错误日志不存在", 404));
        }

        return Ok(ApiResponse<ErrorLog>.Success(log));
    }

    /// <summary>
    /// 更新错误日志状态
    /// </summary>
    [HttpPut("{id}/status")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UpdateStatus(long id, [FromBody] UpdateStatusRequest request)
    {
        var log = await _context.ErrorLogs.FindAsync(id);
        if (log == null)
        {
            return Ok(ApiResponse.Error("错误日志不存在", 404));
        }

        log.Status = request.Status;
        if (request.Status == 1) // 已处理
        {
            log.ResolvedAt = DateTime.Now;
        }
        else
        {
            log.ResolvedAt = null;
        }

        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Success(null, "状态已更新"));
    }

    /// <summary>
    /// 获取错误统计
    /// </summary>
    [HttpGet("stats")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetStats()
    {
        var total = await _context.ErrorLogs.CountAsync();
        var unhandled = await _context.ErrorLogs.CountAsync(e => e.Status == 0);
        var handled = await _context.ErrorLogs.CountAsync(e => e.Status == 1);
        var ignored = await _context.ErrorLogs.CountAsync(e => e.Status == 2);

        var byType = await _context.ErrorLogs
            .GroupBy(e => e.ErrorType)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync();

        var recentErrors = await _context.ErrorLogs
            .Where(e => e.CreatedAt >= DateTime.Now.AddDays(-7))
            .GroupBy(e => e.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync();

        return Ok(ApiResponse.Success(new
        {
            Total = total,
            Unhandled = unhandled,
            Handled = handled,
            Ignored = ignored,
            ByType = byType,
            RecentErrors = recentErrors
        }));
    }

    /// <summary>
    /// 批量更新错误日志状态
    /// </summary>
    [HttpPut("batch/status")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> BatchUpdateStatus([FromBody] BatchUpdateStatusRequest request)
    {
        if (request.Ids == null || request.Ids.Count == 0)
        {
            return Ok(ApiResponse.Error("请选择要处理的错误日志", 400));
        }

        var logs = await _context.ErrorLogs
            .Where(e => request.Ids.Contains(e.Id))
            .ToListAsync();

        if (logs.Count == 0)
        {
            return Ok(ApiResponse.Error("未找到要处理的错误日志", 404));
        }

        foreach (var log in logs)
        {
            log.Status = request.Status;
            if (request.Status == 1) // 已处理
            {
                log.ResolvedAt = DateTime.Now;
            }
            else
            {
                log.ResolvedAt = null;
            }
        }

        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Success(new { UpdatedCount = logs.Count }, $"已更新 {logs.Count} 条错误日志"));
    }

    /// <summary>
    /// 批量删除错误日志
    /// </summary>
    [HttpDelete("batch")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> BatchDelete([FromBody] BatchDeleteRequest request)
    {
        if (request.Ids == null || request.Ids.Count == 0)
        {
            return Ok(ApiResponse.Error("请选择要删除的错误日志", 400));
        }

        var logs = await _context.ErrorLogs
            .Where(e => request.Ids.Contains(e.Id))
            .ToListAsync();

        if (logs.Count == 0)
        {
            return Ok(ApiResponse.Error("未找到要删除的错误日志", 404));
        }

        _context.ErrorLogs.RemoveRange(logs);
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Success(new { DeletedCount = logs.Count }, $"已删除 {logs.Count} 条错误日志"));
    }
}

public class ErrorLogRequest
{
    public string? ErrorType { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorStack { get; set; }
    public string? ErrorUrl { get; set; }
    public string? VisitorId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class UpdateStatusRequest
{
    public sbyte Status { get; set; }
}

public class BatchUpdateStatusRequest
{
    public List<long> Ids { get; set; } = new();
    public sbyte Status { get; set; }
}

public class BatchDeleteRequest
{
    public List<long> Ids { get; set; } = new();
}

