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

    [MaxLength(50)]
    [Column("icon")]
    public string? Icon { get; set; }

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [Column("current_rating", TypeName = "decimal(3,1)")]
    public decimal CurrentRating { get; set; } = 0.0m;

    [Column("target_rating", TypeName = "decimal(3,1)")]
    public decimal? TargetRating { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性（逻辑关联，不使用数据库外键约束）
    // 注意：category_id 通过逻辑关联到 skill_category.id，由应用层维护关联关系
    public SkillCategory? Category { get; set; }
    
    // 技能评级历史
    public List<SkillRating> Ratings { get; set; } = new();
    
    // 学习日志
    public List<LearningLog> LearningLogs { get; set; } = new();
}
