using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 工具分类表
/// </summary>
[Table("tool_category")]
public class ToolCategory
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("slug")]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("icon")]
    public string? Icon { get; set; }

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

