using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

[Table("user_theme_preference")]
public class UserThemePreference
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(36)]
    [Column("visitor_id")]
    public string VisitorId { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("theme_code")]
    public string? ThemeCode { get; set; }

    [MaxLength(50)]
    [Column("background_effect_code")]
    public string? BackgroundEffectCode { get; set; }

    [Column("auto_switch")]
    public bool AutoSwitch { get; set; } = false;

    [Column("switch_interval")]
    public int SwitchInterval { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

