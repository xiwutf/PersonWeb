using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace PersonalSite.Api.Models;

/// <summary>AI 分析结果</summary>
[Table("intelligence_analysis")]
public class IntelligenceAnalysis
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("content_id")]
    public int ContentId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("category")]
    public string Category { get; set; } = string.Empty; // AI技术|软件开发|商业机会|投资理财|认知成长|其他

    [Column("summary", TypeName = "longtext")]
    public string? Summary { get; set; }

    [Column("core_points_json", TypeName = "json")]
    public string? CorePointsJson { get; set; }

    [Column("tags_json", TypeName = "json")]
    public string? TagsJson { get; set; }

    [Column("relevance_score")]
    public int RelevanceScore { get; set; } = 0; // 0-100

    [MaxLength(10)]
    [Column("learning_value")]
    public string LearningValue { get; set; } = "中"; // 高|中|低

    [MaxLength(10)]
    [Column("business_value")]
    public string BusinessValue { get; set; } = "中"; // 高|中|低

    [Column("action_suggestion", TypeName = "longtext")]
    public string? ActionSuggestion { get; set; }

    [MaxLength(100)]
    [Column("model_name")]
    public string? ModelName { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    /// <summary>核心要点列表</summary>
    [NotMapped]
    public List<string> CorePoints
    {
        get => string.IsNullOrEmpty(CorePointsJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(CorePointsJson) ?? new List<string>();
        set => CorePointsJson = value == null ? null : JsonSerializer.Serialize(value);
    }

    /// <summary>标签列表</summary>
    [NotMapped]
    public List<string> Tags
    {
        get => string.IsNullOrEmpty(TagsJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(TagsJson) ?? new List<string>();
        set => TagsJson = value == null ? null : JsonSerializer.Serialize(value);
    }

    /// <summary>内容关联</summary>
    [ForeignKey(nameof(ContentId))]
    public virtual IntelligenceContent Content { get; set; } = null!;
}
