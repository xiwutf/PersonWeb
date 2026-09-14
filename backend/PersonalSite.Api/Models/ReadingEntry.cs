using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 阅读 Inbox 条目（MindTrace 等外部发布）
/// </summary>
[Table("reading_entries")]
public class ReadingEntry
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>来源类型，如 mindtrace</summary>
    [Required]
    [MaxLength(64)]
    [Column("source_type")]
    public string SourceType { get; set; } = "mindtrace";

    /// <summary>外部唯一 ID（MindTrace thought id）</summary>
    [Required]
    [MaxLength(191)]
    [Column("external_id")]
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>摘录</summary>
    [Required]
    [Column("quote", TypeName = "longtext")]
    public string Quote { get; set; } = string.Empty;

    /// <summary>想法</summary>
    [Required]
    [Column("note", TypeName = "longtext")]
    public string Note { get; set; } = string.Empty;

    [MaxLength(512)]
    [Column("source_title")]
    public string SourceTitle { get; set; } = string.Empty;

    [MaxLength(2048)]
    [Column("source_url")]
    public string SourceUrl { get; set; } = string.Empty;

    [Column("source_created_at")]
    public DateTime? SourceCreatedAt { get; set; }

    [Column("imported_at")]
    public DateTime ImportedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    /// <summary>标签 JSON 数组字符串</summary>
    [Column("tags", TypeName = "text")]
    public string? Tags { get; set; }

    /// <summary>inbox | archived | published</summary>
    [Required]
    [MaxLength(32)]
    [Column("status")]
    public string Status { get; set; } = "inbox";

    /// <summary>private | public</summary>
    [Required]
    [MaxLength(32)]
    [Column("visibility")]
    public string Visibility { get; set; } = "private";
}
