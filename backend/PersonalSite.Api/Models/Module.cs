using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

[Table("module")]
public class Module
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

    [MaxLength(20)]
    [Column("module_version")]
    public string ModuleVersion { get; set; } = "1.0.0";

    [Column("description", TypeName = "TEXT")]
    public string? Description { get; set; }

    [MaxLength(100)]
    [Column("author")]
    public string? Author { get; set; }

    [MaxLength(100)]
    [Column("icon")]
    public string? Icon { get; set; }

    [MaxLength(50)]
    [Column("category")]
    public string? Category { get; set; }

    [Column("dependencies", TypeName = "TEXT")]
    public string? Dependencies { get; set; }

    [Column("routes", TypeName = "TEXT")]
    public string? Routes { get; set; }

    [Column("components", TypeName = "TEXT")]
    public string? Components { get; set; }

    [Column("permissions", TypeName = "TEXT")]
    public string? Permissions { get; set; }

    [Column("config_schema", TypeName = "TEXT")]
    public string? ConfigSchema { get; set; }

    [Column("is_enabled")]
    public bool IsEnabled { get; set; } = true;

    [Column("is_core")]
    public bool IsCore { get; set; } = false;

    [Column("sort")]
    public int Sort { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    public virtual ICollection<ModuleConfig> ModuleConfigs { get; set; } = new List<ModuleConfig>();
}

