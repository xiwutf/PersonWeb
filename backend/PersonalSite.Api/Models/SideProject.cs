using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 兼职项目表
/// </summary>
[Table("side_project")]
public class SideProject
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [MaxLength(200)]
    [Column("client_name")]
    public string? ClientName { get; set; }

    [MaxLength(200)]
    [Column("client_contact")]
    public string? ClientContact { get; set; }

    [MaxLength(100)]
    [Column("source")]
    public string? Source { get; set; } // 朋友介绍/平台/公司/其他

    [MaxLength(100)]
    [Column("category")]
    public string? Category { get; set; } // 网站/小程序/AI/其他（软件开发）或 股票/基金/加密货币等（投资）

    [MaxLength(50)]
    [Column("income_type")]
    public string? IncomeType { get; set; } = "development"; // development=软件开发, investment=投资

    [MaxLength(200)]
    [Column("tech_stack")]
    public string? TechStack { get; set; } // 以逗号分隔：Vue,.NET,Python（仅软件开发需要）

    [Column("budget_min", TypeName = "decimal(18,2)")]
    public decimal? BudgetMin { get; set; }

    [Column("budget_max", TypeName = "decimal(18,2)")]
    public decimal? BudgetMax { get; set; }

    [Column("price_final", TypeName = "decimal(18,2)")]
    public decimal? PriceFinal { get; set; }

    [Column("status")]
    public int Status { get; set; } = 0; // 0 进行中 / 1 已完成 / 2 待付款 / 3 已取消

    [Column("start_time", TypeName = "datetime")]
    public DateTime? StartTime { get; set; }

    [Column("end_time", TypeName = "datetime")]
    public DateTime? EndTime { get; set; }

    [Column("is_public")]
    public bool IsPublic { get; set; } = false; // 是否公开展示

    [MaxLength(50)]
    [Column("stage")]
    public string? Stage { get; set; } // 阶段：待开始/进行中/卡住/待验收/已完成

    [Column("progress")]
    public int? Progress { get; set; } // 进度 0-100

    [Column("is_progress_manual")]
    public bool IsProgressManual { get; set; } = false; // 进度是否手动覆盖（false=自动计算，true=手动设置）

    [Column("priority")]
    public int? Priority { get; set; } // 优先级：0=低，1=中，2=高，3=紧急

    [Column("deadline_at", TypeName = "datetime")]
    public DateTime? DeadlineAt { get; set; } // 截止时间

    [MaxLength(500)]
    [Column("next_action")]
    public string? NextAction { get; set; } // 下一步行动

    [Column("blocked")]
    public bool Blocked { get; set; } = false; // 是否阻塞

    [MaxLength(1000)]
    [Column("block_reason")]
    public string? BlockReason { get; set; } // 阻塞原因

    [Column("total_amount", TypeName = "decimal(18,2)")]
    public decimal? TotalAmount { get; set; } // 总金额

    [Column("received_amount", TypeName = "decimal(18,2)")]
    public decimal? ReceivedAmount { get; set; } // 已收款金额

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    public virtual ICollection<SideProjectRequirement> Requirements { get; set; } = new List<SideProjectRequirement>();
    public virtual ICollection<SideProjectTask> Tasks { get; set; } = new List<SideProjectTask>();
    public virtual ICollection<SideProjectMilestone> Milestones { get; set; } = new List<SideProjectMilestone>();
    public virtual ICollection<SideProjectLog> Logs { get; set; } = new List<SideProjectLog>();
    public virtual ICollection<SideProjectAttachment> Attachments { get; set; } = new List<SideProjectAttachment>();
}

