using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 定投执行记录表
/// </summary>
[Table("dca_execution")]
public class DcaExecution
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("dca_plan_id")]
    public long DcaPlanId { get; set; } // 定投计划ID

    [Column("execution_date")]
    public DateTime ExecutionDate { get; set; } = DateTime.Now; // 执行日期

    [Column("amount")]
    public decimal Amount { get; set; } = 0; // 执行金额

    [Column("price")]
    public decimal Price { get; set; } = 0; // 执行时的价格

    [Column("quantity")]
    public decimal Quantity { get; set; } = 0; // 买入数量

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "pending"; // pending / completed / failed

    [Column("error_message", TypeName = "text")]
    public string? ErrorMessage { get; set; } // 错误信息

    [Column("notes", TypeName = "text")]
    public string? Notes { get; set; } // 备注

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("DcaPlanId")]
    public DcaPlan? DcaPlan { get; set; }
}
