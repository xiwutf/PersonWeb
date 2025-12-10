using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 购买前咨询实体类
/// </summary>
[Table("pre_sale_consultations")]
public class PreSaleConsultation
{
    /// <summary>
    /// 咨询ID（主键）
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// 商品ID（逻辑关联到 tool.id）
    /// </summary>
    [Column("product_id")]
    public long ProductId { get; set; }

    /// <summary>
    /// 咨询时商品名称快照
    /// </summary>
    [Required]
    [MaxLength(200)]
    [Column("product_name_snapshot")]
    public string ProductNameSnapshot { get; set; } = string.Empty;

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
    /// 预算范围（如：<500, 500-1000, 1000-3000, 待评估）
    /// </summary>
    [MaxLength(50)]
    [Column("budget_range")]
    public string? BudgetRange { get; set; }

    /// <summary>
    /// 期望完成时间（如：3天内, 一周内, 两周内, 不着急）
    /// </summary>
    [MaxLength(50)]
    [Column("expected_deadline")]
    public string? ExpectedDeadline { get; set; }

    /// <summary>
    /// 需求描述（用户自由描述）
    /// </summary>
    [Required]
    [Column("requirement_description", TypeName = "text")]
    public string RequirementDescription { get; set; } = string.Empty;

    /// <summary>
    /// 咨询状态：0-新咨询, 1-已联系, 2-已转为订单, 3-已关闭/无意向
    /// </summary>
    [Column("status")]
    public int Status { get; set; } = 0; // New = 0

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

    // AI 分析字段
    /// <summary>
    /// AI 生成的摘要
    /// </summary>
    [Column("summary", TypeName = "text")]
    public string? Summary { get; set; }

    /// <summary>
    /// AI 生成的标签（JSON 数组或逗号分隔字符串）
    /// </summary>
    [MaxLength(500)]
    [Column("tags")]
    public string? Tags { get; set; }

    /// <summary>
    /// AI 评分（0-100）
    /// </summary>
    [Column("score")]
    public int? Score { get; set; }

    /// <summary>
    /// AI 推荐建议
    /// </summary>
    [Column("ai_recommendation", TypeName = "text")]
    public string? AiRecommendation { get; set; }
}

/// <summary>
/// 咨询状态枚举
/// </summary>
public enum ConsultationStatus
{
    /// <summary>
    /// 新咨询
    /// </summary>
    New = 0,

    /// <summary>
    /// 已联系
    /// </summary>
    Contacted = 1,

    /// <summary>
    /// 已转为订单
    /// </summary>
    ConvertedToOrder = 2,

    /// <summary>
    /// 已关闭/无意向
    /// </summary>
    Closed = 3
}

