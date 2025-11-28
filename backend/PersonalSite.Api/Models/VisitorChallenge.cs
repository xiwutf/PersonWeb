using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 访客合作挑战表
/// </summary>
[Table("visitor_challenge")]
public class VisitorChallenge
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("challenge_code")]
    public string ChallengeCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("challenge_name")]
    public string ChallengeName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("challenge_type")]
    public string ChallengeType { get; set; } = string.Empty;

    [Column("target_count")]
    public int TargetCount { get; set; }

    [Column("current_count")]
    public int CurrentCount { get; set; } = 0;

    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "active";

    [Column("reward_description", TypeName = "text")]
    public string? RewardDescription { get; set; }

    [Column("unlocked_content", TypeName = "json")]
    public string? UnlockedContent { get; set; }

    [Column("started_at")]
    public DateTime StartedAt { get; set; } = DateTime.Now;

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }
}

