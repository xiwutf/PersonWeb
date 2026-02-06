using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 思维记录表（随手写 + AI 批注）
/// </summary>
[Table("thought_records")]
public class ThoughtRecord
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>原文内容（必填）</summary>
    [Required]
    [Column("content", TypeName = "longtext")]
    public string Content { get; set; } = string.Empty;

    /// <summary>AI 批注（Markdown）</summary>
    [Column("ai_comment", TypeName = "longtext")]
    public string? AiComment { get; set; }

    /// <summary>状态：0 未批注，1 已批注</summary>
    [Required]
    [Column("status")]
    public int Status { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
