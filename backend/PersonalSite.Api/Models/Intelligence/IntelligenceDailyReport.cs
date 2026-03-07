using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>每日情报报告</summary>
[Table("intelligence_daily_report")]
public class IntelligenceDailyReport
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("report_date", TypeName = "date")]
    public DateTime ReportDate { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("content_markdown", TypeName = "longtext")]
    public string? ContentMarkdown { get; set; }

    [Column("item_count")]
    public int ItemCount { get; set; } = 0;

    [Column("generated_at")]
    public DateTime? GeneratedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
