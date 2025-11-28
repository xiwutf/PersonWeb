using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 访客分析扩展表
/// </summary>
[Table("visitor_analytics")]
public class VisitorAnalytics
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [MaxLength(36)]
    [Column("visitor_id")]
    public string? VisitorId { get; set; }

    [MaxLength(45)]
    [Column("ip")]
    public string? Ip { get; set; }

    [MaxLength(100)]
    [Column("country")]
    public string? Country { get; set; }

    [MaxLength(100)]
    [Column("region")]
    public string? Region { get; set; }

    [MaxLength(100)]
    [Column("city")]
    public string? City { get; set; }

    [MaxLength(500)]
    [Column("referrer")]
    public string? Referrer { get; set; } // 来源页面

    [MaxLength(255)]
    [Column("search_keyword")]
    public string? SearchKeyword { get; set; } // 搜索关键词

    [MaxLength(50)]
    [Column("device_type")]
    public string? DeviceType { get; set; } // desktop / mobile / tablet

    [MaxLength(100)]
    [Column("browser")]
    public string? Browser { get; set; }

    [MaxLength(100)]
    [Column("os")]
    public string? Os { get; set; }

    [MaxLength(255)]
    [Column("path")]
    public string? Path { get; set; }

    [Column("session_start")]
    public DateTime SessionStart { get; set; } = DateTime.Now;

    [Column("session_end")]
    public DateTime? SessionEnd { get; set; }

    [Column("page_views")]
    public int PageViews { get; set; } = 1;

    [Column("is_online")]
    public bool IsOnline { get; set; } = true; // 是否在线（最近5分钟有活动）

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

