using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 前端样式规则表
/// </summary>
[Table("frontend_style_rule")]
public class FrontendStyleRule
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("page_key")]
    public string PageKey { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("selector")]
    public string Selector { get; set; } = string.Empty;

    [Required]
    [Column("css_properties", TypeName = "json")]
    public string CssProperties { get; set; } = "{}";

    [Required]
    [Column("priority")]
    public int Priority { get; set; } = 0;

    [Required]
    [Column("enabled")]
    public bool Enabled { get; set; } = true;

    [MaxLength(255)]
    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
