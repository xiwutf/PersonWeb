using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 时间胶囊表
/// </summary>
[Table("time_capsule")]
public class TimeCapsule
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("content", TypeName = "text")]
    public string Content { get; set; } = string.Empty;

    [MaxLength(36)]
    [Column("visitor_id")]
    public string? VisitorId { get; set; }

    [MaxLength(50)]
    [Column("visitor_name")]
    public string? VisitorName { get; set; }

    /// <summary>
    /// 状态：0-待审核 1-已展示 2-已拒绝
    /// </summary>
    [Column("status")]
    public sbyte Status { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

