using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 定投计划表
/// </summary>
[Table("dca_plan")]
public class DcaPlan
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("code")]
    public string Code { get; set; } = string.Empty; // 股票/基金代码

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty; // 名称

    [MaxLength(20)]
    [Column("type")]
    public string Type { get; set; } = "fund"; // stock / fund

    [Column("amount")]
    public decimal Amount { get; set; } = 0; // 每次定投金额

    [MaxLength(20)]
    [Column("frequency")]
    public string Frequency { get; set; } = "monthly"; // daily / weekly / monthly / quarterly

    [Column("next_execution_date")]
    public DateTime? NextExecutionDate { get; set; } // 下次执行日期

    [Column("last_execution_date")]
    public DateTime? LastExecutionDate { get; set; } // 上次执行日期

    [Column("total_executions")]
    public int TotalExecutions { get; set; } = 0; // 总执行次数

    [Column("total_invested")]
    public decimal TotalInvested { get; set; } = 0; // 累计投入金额

    [Column("is_active")]
    public bool IsActive { get; set; } = true; // 是否启用

    [Column("start_date")]
    public DateTime StartDate { get; set; } = DateTime.Now; // 开始日期

    [Column("end_date")]
    public DateTime? EndDate { get; set; } // 结束日期（可选）

    [Column("notes", TypeName = "text")]
    public string? Notes { get; set; } // 备注

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    public List<DcaExecution> Executions { get; set; } = new();
}
