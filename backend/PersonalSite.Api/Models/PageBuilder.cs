using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace PersonalSite.Api.Models;

/// <summary>
/// 用户页面表
/// </summary>
[Table("user_pages")]
public class UserPage
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("slug")]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("domain")]
    public string? Domain { get; set; }

    [Column("template_id")]
    public long? TemplateId { get; set; }

    [Column("layout_config", TypeName = "json")]
    public string? LayoutConfig { get; set; }

    [Column("seo_config", TypeName = "json")]
    public string? SeoConfig { get; set; }

    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "draft";

    [Column("published_at")]
    public DateTime? PublishedAt { get; set; }

    [Column("view_count")]
    public int ViewCount { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    public List<PageComponent> Components { get; set; } = new();
}

/// <summary>
/// 页面组件表
/// </summary>
[Table("page_components")]
public class PageComponent
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("page_id")]
    public long PageId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("component_type")]
    public string ComponentType { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("component_id")]
    public string? ComponentId { get; set; }

    [Column("position")]
    public int Position { get; set; } = 0;

    [Column("config", TypeName = "json")]
    public string? Config { get; set; }

    [Column("style", TypeName = "json")]
    public string? Style { get; set; }

    [Column("parent_id")]
    public long? ParentId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("PageId")]
    public UserPage? Page { get; set; }
}

/// <summary>
/// 组件库表
/// </summary>
[Table("component_library")]
public class ComponentLibrary
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("type")]
    public string Type { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("category")]
    public string? Category { get; set; }

    [Column("config_schema", TypeName = "json")]
    public string? ConfigSchema { get; set; }

    [MaxLength(500)]
    [Column("preview_image")]
    public string? PreviewImage { get; set; }

    [Column("is_premium")]
    public bool IsPremium { get; set; } = false;

    [Column("price", TypeName = "decimal(10,2)")]
    public decimal Price { get; set; } = 0.00m;

    [Column("usage_count")]
    public int UsageCount { get; set; } = 0;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// 页面模板表
/// </summary>
[Table("page_templates")]
public class PageTemplate
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("category")]
    public string? Category { get; set; }

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [MaxLength(500)]
    [Column("preview_image")]
    public string? PreviewImage { get; set; }

    [Column("components", TypeName = "json")]
    public string? Components { get; set; }

    [Column("is_premium")]
    public bool IsPremium { get; set; } = false;

    [Column("price", TypeName = "decimal(10,2)")]
    public decimal Price { get; set; } = 0.00m;

    [Column("usage_count")]
    public int UsageCount { get; set; } = 0;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

