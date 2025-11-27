using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 站点配置表
/// </summary>
[Table("site_config")]
public class SiteConfig
{
    [Key]
    [MaxLength(100)]
    [Column("config_key")]
    public string ConfigKey { get; set; } = string.Empty;

    [Column("config_value", TypeName = "text")]
    public string? ConfigValue { get; set; }

    [MaxLength(255)]
    [Column("description")]
    public string? Description { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
