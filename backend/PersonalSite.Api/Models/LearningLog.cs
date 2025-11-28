using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 学习日志表
/// </summary>
[Table("learning_log")]
public class LearningLog
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("skill_id")]
    public long SkillId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("content", TypeName = "text")]
    public string? Content { get; set; }

    [Column("duration")]
    public int? Duration { get; set; }

    [MaxLength(50)]
    [Column("resource_type")]
    public string? ResourceType { get; set; }

    [MaxLength(500)]
    [Column("resource_url")]
    public string? ResourceUrl { get; set; }

    [Column("learned_at")]
    public DateTime LearnedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("SkillId")]
    public Skill? Skill { get; set; }
}

