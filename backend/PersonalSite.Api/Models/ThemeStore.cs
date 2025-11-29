using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 主题商店表
/// </summary>
[Table("theme_store")]
public class ThemeStore
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
    [Column("preview_image")]
    public string? PreviewImage { get; set; }

    [Column("preview_images", TypeName = "json")]
    public string? PreviewImages { get; set; }

    [Column("price", TypeName = "decimal(10,2)")]
    public decimal Price { get; set; } = 0.00m;

    [Column("is_free")]
    public bool IsFree { get; set; } = false;

    [MaxLength(50)]
    [Column("category")]
    public string? Category { get; set; }

    [Column("author_id")]
    public long? AuthorId { get; set; }

    [MaxLength(100)]
    [Column("author_name")]
    public string? AuthorName { get; set; }

    [Column("download_count")]
    public int DownloadCount { get; set; } = 0;

    [Column("purchase_count")]
    public int PurchaseCount { get; set; } = 0;

    [Column("rating", TypeName = "decimal(3,2)")]
    public decimal Rating { get; set; } = 0.00m;

    [Column("rating_count")]
    public int RatingCount { get; set; } = 0;

    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "draft";

    [Column("tags", TypeName = "json")]
    public string? Tags { get; set; }

    [Column("features", TypeName = "json")]
    public string? Features { get; set; }

    [MaxLength(20)]
    [Column("version")]
    public string Version { get; set; } = "1.0.0";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// 主题文件表
/// </summary>
[Table("theme_files")]
public class ThemeFile
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("theme_id")]
    public long ThemeId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("file_type")]
    public string FileType { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    [Column("file_path")]
    public string FilePath { get; set; } = string.Empty;

    [Column("file_size")]
    public long? FileSize { get; set; }

    [MaxLength(20)]
    [Column("version")]
    public string Version { get; set; } = "1.0.0";

    [MaxLength(64)]
    [Column("checksum")]
    public string? Checksum { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// 主题购买记录表
/// </summary>
[Table("theme_purchases")]
public class ThemePurchase
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [Column("theme_id")]
    public long ThemeId { get; set; }

    [MaxLength(20)]
    [Column("purchase_type")]
    public string PurchaseType { get; set; } = "one_time";

    [Column("price", TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [MaxLength(50)]
    [Column("payment_method")]
    public string? PaymentMethod { get; set; }

    [MaxLength(20)]
    [Column("payment_status")]
    public string PaymentStatus { get; set; } = "pending";

    [MaxLength(200)]
    [Column("payment_id")]
    public string? PaymentId { get; set; }

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("purchased_at")]
    public DateTime PurchasedAt { get; set; } = DateTime.Now;

    [Column("paid_at")]
    public DateTime? PaidAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("ThemeId")]
    public ThemeStore? Theme { get; set; }
}

