using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>采集的情报内容</summary>
[Table("intelligence_content")]
public class IntelligenceContent
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("source_id")]
    public int SourceId { get; set; }

    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    [Column("original_url")]
    public string OriginalUrl { get; set; } = string.Empty;

    [Column("publish_time")]
    public DateTime? PublishTime { get; set; }

    [MaxLength(200)]
    [Column("author")]
    public string? Author { get; set; }

    [Column("raw_text", TypeName = "longtext")]
    public string? RawText { get; set; }

    [Column("clean_text", TypeName = "longtext")]
    public string? CleanText { get; set; }

    [Required]
    [MaxLength(64)]
    [Column("content_hash")]
    public string ContentHash { get; set; } = string.Empty;

    [MaxLength(20)]
    [Column("fetch_status")]
    public string FetchStatus { get; set; } = "pending"; // pending | success | failed

    [MaxLength(20)]
    [Column("analyze_status")]
    public string AnalyzeStatus { get; set; } = "pending"; // pending | processing | success | failed

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    /// <summary>来源关联</summary>
    [ForeignKey(nameof(SourceId))]
    public virtual IntelligenceSource? Source { get; set; }

    /// <summary>分析结果关联</summary>
    public virtual IntelligenceAnalysis? Analysis { get; set; }
}
