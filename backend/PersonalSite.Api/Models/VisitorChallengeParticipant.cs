using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 挑战参与者表
/// </summary>
[Table("visitor_challenge_participant")]
public class VisitorChallengeParticipant
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("challenge_id")]
    public long ChallengeId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("visitor_id")]
    public string VisitorId { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("action_type")]
    public string ActionType { get; set; } = string.Empty;

    [Column("action_data", TypeName = "json")]
    public string? ActionData { get; set; }

    [Column("contributed_count")]
    public int ContributedCount { get; set; } = 1;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("ChallengeId")]
    public VisitorChallenge? Challenge { get; set; }
}

