using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

[Table("theme_setting")]
public class ThemeSetting
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("setting_key")]
    public string SettingKey { get; set; } = string.Empty;

    [Column("setting_value", TypeName = "TEXT")]
    public string? SettingValue { get; set; }

    [MaxLength(255)]
    [Column("description")]
    public string? Description { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

