using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

[Table("style_definition")]
public class StyleDefinition
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("category_id")]
    public long CategoryId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("css_class")]
    public string CssClass { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("background_color")]
    public string? BackgroundColor { get; set; }

    [MaxLength(50)]
    [Column("border_color")]
    public string? BorderColor { get; set; }

    [MaxLength(50)]
    [Column("text_color")]
    public string? TextColor { get; set; }

    [MaxLength(20)]
    [Column("font_size")]
    public string? FontSize { get; set; }

    [MaxLength(20)]
    [Column("font_weight")]
    public string? FontWeight { get; set; }

    [MaxLength(20)]
    [Column("padding")]
    public string? Padding { get; set; }

    [MaxLength(20)]
    [Column("border_radius")]
    public string? BorderRadius { get; set; }

    [MaxLength(20)]
    [Column("border_width")]
    public string? BorderWidth { get; set; }

    [Column("custom_css", TypeName = "TEXT")]
    public string? CustomCss { get; set; }

    [MaxLength(255)]
    [Column("description")]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("sort")]
    public int Sort { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("CategoryId")]
    public virtual StyleCategory? Category { get; set; }

    public virtual ICollection<StyleUsage> StyleUsages { get; set; } = new List<StyleUsage>();
}

