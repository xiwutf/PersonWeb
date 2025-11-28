using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

[Table("admin_global_style")]
public class AdminGlobalStyle
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("style_key")]
    public string StyleKey { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("style_name")]
    public string StyleName { get; set; } = string.Empty;

    [Column("description", TypeName = "TEXT")]
    public string? Description { get; set; }

    [Required]
    [Column("style_config", TypeName = "JSON")]
    public string StyleConfig { get; set; } = "{}";

    [MaxLength(255)]
    [Column("preview_image")]
    public string? PreviewImage { get; set; }

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

