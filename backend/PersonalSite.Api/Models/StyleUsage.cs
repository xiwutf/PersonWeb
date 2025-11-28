using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

[Table("style_usage")]
public class StyleUsage
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("style_id")]
    public long StyleId { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("page_path")]
    public string PagePath { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("component_name")]
    public string? ComponentName { get; set; }

    [Column("usage_count")]
    public int UsageCount { get; set; } = 1;

    [Column("last_used_at")]
    public DateTime? LastUsedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("StyleId")]
    public virtual StyleDefinition? Style { get; set; }
}

