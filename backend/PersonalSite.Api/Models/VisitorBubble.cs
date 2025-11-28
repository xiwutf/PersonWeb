using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

[Table("visitor_bubble")]
public class VisitorBubble
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("visitor_id")]
    public string VisitorId { get; set; } = string.Empty;

    [MaxLength(255)]
    [Column("avatar_url")]
    public string? AvatarUrl { get; set; }

    [MaxLength(100)]
    [Column("location")]
    public string? Location { get; set; }

    [MaxLength(100)]
    [Column("display_text")]
    public string? DisplayText { get; set; }

    [Column("displayed_at")]
    public DateTime DisplayedAt { get; set; } = DateTime.Now;
}

