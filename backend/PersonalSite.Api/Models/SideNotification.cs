using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PersonalSite.Api.Models.Enums;

namespace PersonalSite.Api.Models;

/// <summary>
/// 副业项目站内提醒表
/// </summary>
[Table("side_notification")]
public class SideNotification
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// 用户ID（可先固定为空或默认管理员）
    /// </summary>
    [Column("user_id")]
    public int? UserId { get; set; }

    /// <summary>
    /// 提醒类型
    /// </summary>
    [Column("type")]
    public NotificationType Type { get; set; }

    /// <summary>
    /// 提醒标题
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 提醒内容
    /// </summary>
    [MaxLength(500)]
    [Column("content")]
    public string? Content { get; set; }

    /// <summary>
    /// 严重程度
    /// </summary>
    [Column("severity")]
    public NotificationSeverity Severity { get; set; } = NotificationSeverity.Info;

    /// <summary>
    /// 实体类型（SideProject / SideProjectTask）
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("entity_type")]
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// 实体ID
    /// </summary>
    [Column("entity_id")]
    public long EntityId { get; set; }

    /// <summary>
    /// 负载数据（JSON格式）
    /// </summary>
    [Column("payload_json", TypeName = "text")]
    public string? PayloadJson { get; set; }

    /// <summary>
    /// 是否已读
    /// </summary>
    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// 已读时间
    /// </summary>
    [Column("read_at", TypeName = "datetime")]
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// 是否已忽略
    /// </summary>
    [Column("is_dismissed")]
    public bool IsDismissed { get; set; } = false;

    /// <summary>
    /// 忽略时间
    /// </summary>
    [Column("dismissed_at", TypeName = "datetime")]
    public DateTime? DismissedAt { get; set; }

    /// <summary>
    /// 延后到某个时间再出现
    /// </summary>
    [Column("snooze_until", TypeName = "datetime")]
    public DateTime? SnoozeUntil { get; set; }

    /// <summary>
    /// 发生日期（用于去重，同一天同实体同类型只一条）
    /// </summary>
    [Column("occur_date", TypeName = "date")]
    public DateOnly OccurDate { get; set; }

    /// <summary>
    /// 首次触发时间
    /// </summary>
    [Column("first_triggered_at", TypeName = "datetime")]
    public DateTime FirstTriggeredAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 最后触发时间
    /// </summary>
    [Column("last_triggered_at", TypeName = "datetime")]
    public DateTime LastTriggeredAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 触发次数
    /// </summary>
    [Column("trigger_count")]
    public int TriggerCount { get; set; } = 1;

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

