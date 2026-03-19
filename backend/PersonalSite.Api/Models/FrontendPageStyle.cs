using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 前端页面样式配置表
/// </summary>
[Table("frontend_page_style")]
public class FrontendPageStyle
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("page_key")]
    public string PageKey { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("page_name")]
    public string PageName { get; set; } = string.Empty;

    [Required]
    [Column("style_config", TypeName = "json")]
    public string StyleConfig { get; set; } = "{}";

    [Required]
    [Column("enabled")]
    public bool Enabled { get; set; } = true;

    [Required]
    [Column("is_default")]
    public bool IsDefault { get; set; } = false;

    [Required]
    [Column("version")]
    public int Version { get; set; } = 1;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
