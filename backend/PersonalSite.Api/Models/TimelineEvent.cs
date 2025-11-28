using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 成长轨迹时间线事件表
/// </summary>
[Table("timeline_event")]
public class TimelineEvent
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("year")]
    public int Year { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [MaxLength(50)]
    [Column("icon")]
    public string? Icon { get; set; } // emoji 或图标类名

    [MaxLength(50)]
    [Column("color")]
    public string? Color { get; set; } // 颜色主题

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    [Column("status")]
    public sbyte Status { get; set; } = 1; // 0-隐藏 1-显示

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

