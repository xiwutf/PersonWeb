using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 技能表
/// </summary>
[Table("skill")]
public class Skill
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("category_id")]
    public long CategoryId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [MaxLength(50)]
    [Column("icon")]
    public string? Icon { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("CategoryId")]
    public SkillCategory? Category { get; set; }

    public List<SkillRating> Ratings { get; set; } = new();
    public List<LearningLog> LearningLogs { get; set; } = new();
}

