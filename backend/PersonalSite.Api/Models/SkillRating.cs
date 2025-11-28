using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 技能评级表
/// </summary>
[Table("skill_rating")]
public class SkillRating
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("skill_id")]
    public long SkillId { get; set; }

    [Column("rating", TypeName = "decimal(3,1)")]
    public decimal Rating { get; set; }

    [Column("notes", TypeName = "text")]
    public string? Notes { get; set; }

    [Column("recorded_at")]
    public DateTime RecordedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("SkillId")]
    public Skill? Skill { get; set; }
}

