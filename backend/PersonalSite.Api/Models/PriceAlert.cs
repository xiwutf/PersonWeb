using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 价格提醒表
/// </summary>
[Table("price_alert")]
public class PriceAlert
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("code")]
    public string Code { get; set; } = string.Empty; // 股票/基金代码

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty; // 名称

    [MaxLength(20)]
    [Column("type")]
    public string Type { get; set; } = "fund"; // stock / fund

    [Column("target_price")]
    public decimal TargetPrice { get; set; } = 0; // 目标价格

    [MaxLength(20)]
    [Column("alert_type")]
    public string AlertType { get; set; } = "above"; // above / below / both

    [Column("current_price")]
    public decimal CurrentPrice { get; set; } = 0; // 当前价格

    [Column("is_triggered")]
    public bool IsTriggered { get; set; } = false; // 是否已触发

    [Column("triggered_at")]
    public DateTime? TriggeredAt { get; set; } // 触发时间

    [Column("is_active")]
    public bool IsActive { get; set; } = true; // 是否启用

    [Column("notification_sent")]
    public bool NotificationSent { get; set; } = false; // 是否已发送通知

    [Column("notes", TypeName = "text")]
    public string? Notes { get; set; } // 备注

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
