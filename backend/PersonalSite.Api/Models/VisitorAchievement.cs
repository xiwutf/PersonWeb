using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 访客成就表
/// </summary>
[Table("visitor_achievement")]
public class VisitorAchievement
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
    [Column("achievement_code")]
    public string AchievementCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("achievement_name")]
    public string AchievementName { get; set; } = string.Empty;

    [Column("achievement_description", TypeName = "text")]
    public string? AchievementDescription { get; set; }

    [MaxLength(100)]
    [Column("icon")]
    public string? Icon { get; set; }

    [Column("unlocked_at")]
    public DateTime UnlockedAt { get; set; } = DateTime.Now;
}

