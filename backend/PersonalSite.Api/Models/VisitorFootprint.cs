using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

[Table("visitor_footprint")]
public class VisitorFootprint
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("visitor_id")]
    public string VisitorId { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    [Column("emoji")]
    public string Emoji { get; set; } = string.Empty;

    [MaxLength(20)]
    [Column("icon_type")]
    public string IconType { get; set; } = "emoji"; // emoji, icon

    [MaxLength(200)]
    [Column("message")]
    public string? Message { get; set; }

    [Column("x_position", TypeName = "DECIMAL(5,2)")]
    public decimal? XPosition { get; set; }

    [Column("y_position", TypeName = "DECIMAL(5,2)")]
    public decimal? YPosition { get; set; }

    [MaxLength(50)]
    [Column("ip")]
    public string? Ip { get; set; }

    [MaxLength(100)]
    [Column("location")]
    public string? Location { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

