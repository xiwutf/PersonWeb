using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 用户行为记录表（用于 AI 推荐）
/// </summary>
[Table("user_behavior")]
public class UserBehavior
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [MaxLength(36)]
    [Column("user_id")]
    public string? UserId { get; set; } // 用户ID（可以是访客ID或管理员ID）

    [MaxLength(50)]
    [Column("behavior_type")]
    public string BehaviorType { get; set; } = string.Empty; // read_article, view_book, add_tag, etc.

    [MaxLength(255)]
    [Column("target_id")]
    public string? TargetId { get; set; } // 目标ID（文章ID、书籍ID等）

    [MaxLength(500)]
    [Column("target_title")]
    public string? TargetTitle { get; set; } // 目标标题

    [MaxLength(500)]
    [Column("tags")]
    public string? Tags { get; set; } // 相关标签

    [MaxLength(500)]
    [Column("category")]
    public string? Category { get; set; } // 分类

    [Column("duration")]
    public int? Duration { get; set; } // 停留时长（秒）

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

