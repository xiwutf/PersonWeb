using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 通用内容评论（target_type + target_id）
/// </summary>
[Table("comments")]
public class ContentComment
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(32)]
    [Column("target_type")]
    public string TargetType { get; set; } = string.Empty;

    [Required]
    [MaxLength(180)]
    [Column("target_id")]
    public string TargetId { get; set; } = string.Empty;

    /// <summary>V1 不启用回复，保留扩展字段</summary>
    [Column("parent_id")]
    public long? ParentId { get; set; }

    [Required]
    [MaxLength(30)]
    [Column("nickname")]
    public string Nickname { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("content", TypeName = "TEXT")]
    public string Content { get; set; } = string.Empty;

    /// <summary>pending / approved / rejected</summary>
    [Required]
    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "pending";

    [MaxLength(50)]
    [Column("ip")]
    public string? Ip { get; set; }

    [MaxLength(100)]
    [Column("visitor_key")]
    public string? VisitorKey { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
