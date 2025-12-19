using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Models.Enums;
using PersonalSite.Api.Services;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 站内提醒控制器
/// </summary>
[ApiController]
[Route("api/side-notifications")]
public class SideNotificationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly NotificationService _notificationService;
    private readonly ILogger<SideNotificationsController> _logger;

    public SideNotificationsController(
        AppDbContext context,
        NotificationService notificationService,
        ILogger<SideNotificationsController> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// 获取提醒列表（分页、筛选）
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<NotificationListResponseDto>>> GetList(
        [FromQuery] string? status = null,
        [FromQuery] string? severity = null,
        [FromQuery] NotificationType? type = null,
        [FromQuery] string? keyword = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.SideNotifications.AsQueryable();

        // 状态筛选
        if (!string.IsNullOrEmpty(status))
        {
            switch (status.ToLower())
            {
                case "unread":
                    query = query.Where(n => !n.IsRead && !n.IsDismissed);
                    break;
                case "dismissed":
                    query = query.Where(n => n.IsDismissed);
                    break;
                case "all":
                default:
                    // 不过滤
                    break;
            }
        }
        else
        {
            // 默认只显示未读且未忽略的
            query = query.Where(n => !n.IsRead && !n.IsDismissed);
        }

        // 过滤延后的提醒（如果延后时间未到，不显示）
        var now = DateTime.Now;
        query = query.Where(n => n.SnoozeUntil == null || n.SnoozeUntil <= now);

        // 严重程度筛选
        if (!string.IsNullOrEmpty(severity))
        {
            if (Enum.TryParse<NotificationSeverity>(severity, true, out var severityEnum))
            {
                query = query.Where(n => n.Severity == severityEnum);
            }
        }

        // 类型筛选
        if (type.HasValue)
        {
            query = query.Where(n => n.Type == type.Value);
        }

        // 关键字搜索（标题/内容）
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(n => n.Title.Contains(keyword) || (n.Content != null && n.Content.Contains(keyword)));
        }

        // 获取总数
        var total = await query.CountAsync();

        // 分页
        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new SideNotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Type = n.Type,
                Title = n.Title,
                Content = n.Content,
                Severity = n.Severity,
                EntityType = n.EntityType,
                EntityId = n.EntityId,
                PayloadJson = n.PayloadJson,
                IsRead = n.IsRead,
                ReadAt = n.ReadAt,
                IsDismissed = n.IsDismissed,
                DismissedAt = n.DismissedAt,
                SnoozeUntil = n.SnoozeUntil,
                OccurDate = n.OccurDate,
                FirstTriggeredAt = n.FirstTriggeredAt,
                LastTriggeredAt = n.LastTriggeredAt,
                TriggerCount = n.TriggerCount,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync();

        // 获取未读数量
        var unreadCount = await _context.SideNotifications
            .Where(n => !n.IsRead && !n.IsDismissed && (n.SnoozeUntil == null || n.SnoozeUntil <= now))
            .CountAsync();

        var response = new NotificationListResponseDto
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize,
            UnreadCount = unreadCount
        };

        return Ok(ApiResponse.Success(response));
    }

    /// <summary>
    /// 获取未读数量
    /// </summary>
    [HttpGet("unread-count")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetUnreadCount()
    {
        var now = DateTime.Now;
        var count = await _context.SideNotifications
            .Where(n => !n.IsRead && !n.IsDismissed && (n.SnoozeUntil == null || n.SnoozeUntil <= now))
            .CountAsync();

        return Ok(ApiResponse.Success(new { count }));
    }

    /// <summary>
    /// 标记提醒为已读
    /// </summary>
    [HttpPost("{id}/read")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> MarkAsRead(long id)
    {
        var notification = await _context.SideNotifications.FindAsync(id);
        if (notification == null)
        {
            return NotFound(ApiResponse.Error("提醒不存在", 404));
        }

        notification.IsRead = true;
        notification.ReadAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(null, "已标记为已读"));
    }

    /// <summary>
    /// 一键全部已读
    /// </summary>
    [HttpPost("read-all")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> MarkAllAsRead()
    {
        var now = DateTime.Now;
        var notifications = await _context.SideNotifications
            .Where(n => !n.IsRead && !n.IsDismissed && (n.SnoozeUntil == null || n.SnoozeUntil <= now))
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(new { count = notifications.Count }, "已全部标记为已读"));
    }

    /// <summary>
    /// 忽略提醒
    /// </summary>
    [HttpPost("{id}/dismiss")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> Dismiss(long id)
    {
        var notification = await _context.SideNotifications.FindAsync(id);
        if (notification == null)
        {
            return NotFound(ApiResponse.Error("提醒不存在", 404));
        }

        notification.IsDismissed = true;
        notification.DismissedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(null, "已忽略"));
    }

    /// <summary>
    /// 延后提醒
    /// </summary>
    [HttpPost("{id}/snooze")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> Snooze(long id, [FromBody] SnoozeNotificationDto dto)
    {
        var notification = await _context.SideNotifications.FindAsync(id);
        if (notification == null)
        {
            return NotFound(ApiResponse.Error("提醒不存在", 404));
        }

        DateTime? snoozeUntil = null;

        if (dto.SnoozeUntil.HasValue)
        {
            snoozeUntil = dto.SnoozeUntil.Value;
        }
        else if (!string.IsNullOrEmpty(dto.Preset))
        {
            var now = DateTime.Now;
            switch (dto.Preset.ToLower())
            {
                case "1d":
                    snoozeUntil = now.AddDays(1);
                    break;
                case "3d":
                    snoozeUntil = now.AddDays(3);
                    break;
                case "nextmon":
                    // 下周一
                    var daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
                    if (daysUntilMonday == 0) daysUntilMonday = 7; // 如果今天是周一，延后到下周一
                    snoozeUntil = now.AddDays(daysUntilMonday).Date.AddHours(9); // 下周一早上9点
                    break;
                default:
                    return BadRequest(ApiResponse.Error("无效的预设选项", 400));
            }
        }
        else
        {
            return BadRequest(ApiResponse.Error("必须提供 snoozeUntil 或 preset", 400));
        }

        notification.SnoozeUntil = snoozeUntil;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(new { snoozeUntil }, "已延后"));
    }

    /// <summary>
    /// 手动触发一次提醒生成（仅管理员调试用）
    /// </summary>
    [HttpPost("run-generator")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> RunGenerator()
    {
        try
        {
            await _notificationService.GenerateNotificationsAsync();
            return Ok(ApiResponse.Success(null, "提醒生成完成"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "手动触发提醒生成失败");
            return StatusCode(500, ApiResponse.Error("提醒生成失败: " + ex.Message, 500));
        }
    }
}

