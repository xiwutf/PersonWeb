using PersonalSite.Api.Models.Enums;

namespace PersonalSite.Api.Models.Dto;

/// <summary>
/// 站内提醒 DTO
/// </summary>
public class SideNotificationDto
{
    public long Id { get; set; }
    public int? UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public NotificationSeverity Severity { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public long EntityId { get; set; }
    public string? PayloadJson { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsDismissed { get; set; }
    public DateTime? DismissedAt { get; set; }
    public DateTime? SnoozeUntil { get; set; }
    public DateOnly OccurDate { get; set; }
    public DateTime FirstTriggeredAt { get; set; }
    public DateTime LastTriggeredAt { get; set; }
    public int TriggerCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 提醒列表查询参数 DTO
/// </summary>
public class NotificationQueryDto
{
    /// <summary>
    /// 状态筛选：unread=未读, all=全部, dismissed=已忽略
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 严重程度筛选：info/warning/danger
    /// </summary>
    public string? Severity { get; set; }

    /// <summary>
    /// 类型筛选
    /// </summary>
    public NotificationType? Type { get; set; }

    /// <summary>
    /// 关键字搜索（标题/内容）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 页码
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 提醒列表响应 DTO
/// </summary>
public class NotificationListResponseDto
{
    public List<SideNotificationDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int UnreadCount { get; set; }
}

/// <summary>
/// 延后提醒请求 DTO
/// </summary>
public class SnoozeNotificationDto
{
    /// <summary>
    /// 延后到指定时间（可选）
    /// </summary>
    public DateTime? SnoozeUntil { get; set; }

    /// <summary>
    /// 预设延后选项：1d=1天, 3d=3天, nextMon=下周一（可选）
    /// </summary>
    public string? Preset { get; set; }
}

