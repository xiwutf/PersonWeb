using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace PersonalSite.Api.Models;

/// <summary>
/// 会员类型表
/// </summary>
[Table("membership_types")]
public class MembershipType
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Column("price", TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Column("duration_days")]
    public int? DurationDays { get; set; }

    [Column("features", TypeName = "json")]
    public string? Features { get; set; }

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// 用户会员表
/// </summary>
[Table("user_memberships")]
public class UserMembership
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [Column("membership_type_id")]
    public long MembershipTypeId { get; set; }

    [Column("start_date")]
    public DateTime StartDate { get; set; } = DateTime.Now;

    [Column("end_date")]
    public DateTime? EndDate { get; set; }

    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "active";

    [Column("auto_renew")]
    public bool AutoRenew { get; set; } = false;

    [Column("purchased_at")]
    public DateTime PurchasedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("MembershipTypeId")]
    public MembershipType? MembershipType { get; set; }
}

/// <summary>
/// 付费内容表
/// </summary>
[Table("paid_content")]
public class PaidContent
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("content_type")]
    public string ContentType { get; set; } = string.Empty; // article/video/tool/resource

    [Column("content_id")]
    public long ContentId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("price", TypeName = "decimal(10,2)")]
    public decimal Price { get; set; } = 0.00m;

    [Column("is_member_only")]
    public bool IsMemberOnly { get; set; } = false;

    [MaxLength(50)]
    [Column("required_membership")]
    public string? RequiredMembership { get; set; }

    [Column("unlock_requirements", TypeName = "json")]
    public string? UnlockRequirements { get; set; }

    [Column("preview_content", TypeName = "text")]
    public string? PreviewContent { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// 内容购买记录表
/// </summary>
[Table("content_purchases")]
public class ContentPurchase
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [Column("content_id")]
    public long ContentId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("content_type")]
    public string ContentType { get; set; } = string.Empty;

    [Column("price", TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [MaxLength(20)]
    [Column("purchase_type")]
    public string PurchaseType { get; set; } = "one_time";

    [MaxLength(50)]
    [Column("payment_method")]
    public string? PaymentMethod { get; set; }

    [MaxLength(20)]
    [Column("payment_status")]
    public string PaymentStatus { get; set; } = "pending";

    [MaxLength(200)]
    [Column("payment_id")]
    public string? PaymentId { get; set; }

    [Column("purchased_at")]
    public DateTime PurchasedAt { get; set; } = DateTime.Now;
}

