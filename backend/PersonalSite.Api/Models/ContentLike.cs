using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 内容点赞记录：同一 visitor 对同一 target 只能点一次
/// </summary>
[Table("content_likes")]
public class ContentLike
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

    [Required]
    [MaxLength(100)]
    [Column("visitor_key")]
    public string VisitorKey { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
