using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 工具购买记录表
/// </summary>
[Table("tool_purchase")]
public class ToolPurchase
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("tool_id")]
    public long ToolId { get; set; }

    [MaxLength(100)]
    [Column("user_id")]
    public string? UserId { get; set; }

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

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("purchased_at")]
    public DateTime PurchasedAt { get; set; } = DateTime.Now;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("paid_at")]
    public DateTime? PaidAt { get; set; }

    // 导航属性
    [ForeignKey("ToolId")]
    public Tool? Tool { get; set; }
}

