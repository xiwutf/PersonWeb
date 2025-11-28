using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

[Table("style_category")]
public class StyleCategory
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [MaxLength(255)]
    [Column("description")]
    public string? Description { get; set; }

    [Column("sort")]
    public int Sort { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    public virtual ICollection<StyleDefinition> StyleDefinitions { get; set; } = new List<StyleDefinition>();
}

