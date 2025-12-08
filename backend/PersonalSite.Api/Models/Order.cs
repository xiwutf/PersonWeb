using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 订单实体类
/// </summary>
[Table("orders")]
public class Order
{
    /// <summary>
    /// 订单ID（主键）
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// 订单编号（格式：日期-随机数，如：20251208-XXXX）
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("order_no")]
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 商品ID（逻辑关联到 tool.id）
    /// </summary>
    [Column("product_id")]
    public long ProductId { get; set; }

    /// <summary>
    /// 下单时商品名称快照
    /// </summary>
    [Required]
    [MaxLength(200)]
    [Column("product_name_snapshot")]
    public string ProductNameSnapshot { get; set; } = string.Empty;

    /// <summary>
    /// 下单时价格快照（为空表示面议）
    /// </summary>
    [Column("price_snapshot", TypeName = "decimal(18,2)")]
    public decimal? PriceSnapshot { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [Column("quantity")]
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// 总金额
    /// </summary>
    [Column("total_amount", TypeName = "decimal(18,2)")]
    public decimal? TotalAmount { get; set; }

    /// <summary>
    /// 客户姓名
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("customer_name")]
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 客户手机号
    /// </summary>
    [MaxLength(50)]
    [Column("customer_phone")]
    public string? CustomerPhone { get; set; }

    /// <summary>
    /// 客户微信号
    /// </summary>
    [MaxLength(50)]
    [Column("customer_wechat")]
    public string? CustomerWeChat { get; set; }

    /// <summary>
    /// 客户邮箱
    /// </summary>
    [MaxLength(100)]
    [Column("customer_email")]
    public string? CustomerEmail { get; set; }

    /// <summary>
    /// 需求说明/备注
    /// </summary>
    [Column("remark", TypeName = "text")]
    public string? Remark { get; set; }

    /// <summary>
    /// 订单状态：0-待确认, 1-进行中, 2-已完成, 3-已关闭
    /// </summary>
    [Column("status")]
    public int Status { get; set; } = 0; // PendingReview = 0

    /// <summary>
    /// 内部备注（后台使用）
    /// </summary>
    [Column("internal_note", TypeName = "text")]
    public string? InternalNote { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// 订单状态枚举
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// 待确认
    /// </summary>
    PendingReview = 0,

    /// <summary>
    /// 进行中
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 2,

    /// <summary>
    /// 已关闭
    /// </summary>
    Closed = 3
}

