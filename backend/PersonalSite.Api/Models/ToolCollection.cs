using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 工具合集表
/// </summary>
[Table("tool_collection")]
public class ToolCollection
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("slug")]
    public string Slug { get; set; } = string.Empty;

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [MaxLength(500)]
    [Column("cover_image")]
    public string? CoverImage { get; set; }

    [Column("price", TypeName = "decimal(10,2)")]
    public decimal Price { get; set; } = 0.00m;

    [Column("original_price", TypeName = "decimal(10,2)")]
    public decimal? OriginalPrice { get; set; }

    [Column("tool_count")]
    public int ToolCount { get; set; } = 0;

    [Column("purchase_count")]
    public int PurchaseCount { get; set; } = 0;

    [Column("is_featured")]
    public bool IsFeatured { get; set; } = false;

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "draft";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

