using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 触发事件记录表
/// </summary>
[Table("visitor_trigger_event")]
public class VisitorTriggerEvent
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
    [Column("trigger_type")]
    public string TriggerType { get; set; } = string.Empty;

    [Column("trigger_context", TypeName = "json")]
    public string? TriggerContext { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

