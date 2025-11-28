using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 访客等级表
/// </summary>
[Table("visitor_level")]
public class VisitorLevel
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("visitor_id")]
    public string VisitorId { get; set; } = string.Empty;

    [Column("level")]
    public int Level { get; set; } = 1;

    [Column("experience")]
    public int Experience { get; set; } = 0;

    [Column("total_experience")]
    public int TotalExperience { get; set; } = 0;

    [MaxLength(50)]
    [Column("title")]
    public string? Title { get; set; }

    [MaxLength(50)]
    [Column("badge")]
    public string? Badge { get; set; }

    [Column("unlocked_features", TypeName = "json")]
    public string? UnlockedFeatures { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

