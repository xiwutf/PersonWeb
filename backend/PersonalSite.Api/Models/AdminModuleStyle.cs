using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

[Table("admin_module_style")]
public class AdminModuleStyle
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("module_key")]
    public string ModuleKey { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("module_name")]
    public string ModuleName { get; set; } = string.Empty;

    [Required]
    [Column("style_config", TypeName = "JSON")]
    public string StyleConfig { get; set; } = "{}";

    [Required]
    [Column("enabled")]
    public bool Enabled { get; set; } = true;

    [Required]
    [Column("is_default")]
    public bool IsDefault { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

