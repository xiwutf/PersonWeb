using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 访客行为记录表
/// </summary>
[Table("visitor_behavior")]
public class VisitorBehavior
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("visitor_id")]
    public string VisitorId { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("behavior_type")]
    public string BehaviorType { get; set; } = string.Empty;

    [Column("behavior_data", TypeName = "json")]
    public string? BehaviorData { get; set; }

    [Column("experience_gained")]
    public int ExperienceGained { get; set; } = 0;

    [MaxLength(50)]
    [Column("triggered_event")]
    public string? TriggeredEvent { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

