using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 工具表
/// </summary>
[Table("tool")]
public class Tool
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("category_id")]
    public long? CategoryId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("slug")]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("icon")]
    public string? Icon { get; set; }

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [Column("detailed_description", TypeName = "longtext")]
    public string? DetailedDescription { get; set; }

    [MaxLength(500)]
    [Column("cover_image")]
    public string? CoverImage { get; set; }

    [MaxLength(500)]
    [Column("demo_url")]
    public string? DemoUrl { get; set; }

    [Column("price", TypeName = "decimal(10,2)")]
    public decimal Price { get; set; } = 0.00m;

    [Column("original_price", TypeName = "decimal(10,2)")]
    public decimal? OriginalPrice { get; set; }

    [Column("is_free")]
    public bool IsFree { get; set; } = false;

    [Column("is_premium")]
    public bool IsPremium { get; set; } = false;

    /// <summary>
    /// 是否支持在线下单
    /// </summary>
    [Column("enable_online_order")]
    public bool EnableOnlineOrder { get; set; } = false;

    [Column("purchase_count")]
    public int PurchaseCount { get; set; } = 0;

    [Column("use_count")]
    public int UseCount { get; set; } = 0;

    [Column("rating", TypeName = "decimal(3,2)")]
    public decimal Rating { get; set; } = 0.00m;

    [Column("rating_count")]
    public int RatingCount { get; set; } = 0;

    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "draft";

    [MaxLength(500)]
    [Column("api_endpoint")]
    public string? ApiEndpoint { get; set; }

    [Column("api_documentation", TypeName = "text")]
    public string? ApiDocumentation { get; set; }

    [Column("tags", TypeName = "json")]
    public string? Tags { get; set; }

    [Column("features", TypeName = "json")]
    public string? Features { get; set; }

    [Column("requirements", TypeName = "text")]
    public string? Requirements { get; set; }

    /// <summary>
    /// 适合人群（文本或JSON）
    /// </summary>
    [Column("fit_for", TypeName = "text")]
    public string? FitFor { get; set; }

    /// <summary>
    /// 不适合情况（文本或JSON）
    /// </summary>
    [Column("not_fit_for", TypeName = "text")]
    public string? NotFitFor { get; set; }

    /// <summary>
    /// 交付类型（如：即时交付、定制开发等）
    /// </summary>
    [MaxLength(50)]
    [Column("delivery_type")]
    public string? DeliveryType { get; set; }

    /// <summary>
    /// 预计交付时间（如：1-3天、一周内等）
    /// </summary>
    [MaxLength(100)]
    [Column("estimated_delivery_time")]
    public string? EstimatedDeliveryTime { get; set; }

    [MaxLength(50)]
    [Column("version")]
    public string Version { get; set; } = "1.0.0";

    [MaxLength(100)]
    [Column("author")]
    public string? Author { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [Column("published_at")]
    public DateTime? PublishedAt { get; set; }

    // 导航属性
    [ForeignKey("CategoryId")]
    public ToolCategory? Category { get; set; }
}

