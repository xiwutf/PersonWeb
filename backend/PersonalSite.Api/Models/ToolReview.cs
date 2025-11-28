using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 工具评价表
/// </summary>
[Table("tool_review")]
public class ToolReview
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("tool_id")]
    public long ToolId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [Column("rating")]
    public int Rating { get; set; }

    [Column("comment", TypeName = "text")]
    public string? Comment { get; set; }

    [Column("is_verified_purchase")]
    public bool IsVerifiedPurchase { get; set; } = false;

    [Column("helpful_count")]
    public int HelpfulCount { get; set; } = 0;

    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "pending";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("ToolId")]
    public Tool? Tool { get; set; }
}

