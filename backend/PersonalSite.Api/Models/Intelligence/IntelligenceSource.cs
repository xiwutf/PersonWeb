using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace PersonalSite.Api.Models;

/// <summary>情报来源配置</summary>
[Table("intelligence_source")]
public class IntelligenceSource
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("source_name")]
    public string SourceName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("source_type")]
    public string SourceType { get; set; } = string.Empty; // RSS | WEB

    [Required]
    [MaxLength(500)]
    [Column("source_url")]
    public string SourceUrl { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("category")]
    public string Category { get; set; } = string.Empty;

    [Column("tags_json", TypeName = "json")]
    public string? TagsJson { get; set; }

    [Column("priority")]
    public int Priority { get; set; } = 0;

    [Column("enabled")]
    public bool Enabled { get; set; } = true;

    [Column("fetch_interval_minutes")]
    public int FetchIntervalMinutes { get; set; } = 60; // 默认每小时

    [MaxLength(500)]
    [Column("remark")]
    public string? Remark { get; set; }

    [Column("last_fetch_time")]
    public DateTime? LastFetchTime { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    /// <summary>标签列表</summary>
    [NotMapped]
    public List<string> Tags
    {
        get => string.IsNullOrEmpty(TagsJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(TagsJson) ?? new List<string>();
        set => TagsJson = value == null ? null : JsonSerializer.Serialize(value);
    }
}
