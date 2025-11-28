using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 工具分析统计表
/// </summary>
[Table("tool_analytics")]
public class ToolAnalytics
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("tool_id")]
    public long ToolId { get; set; }

    [Column("date", TypeName = "date")]
    public DateTime Date { get; set; }

    [Column("view_count")]
    public int ViewCount { get; set; } = 0;

    [Column("use_count")]
    public int UseCount { get; set; } = 0;

    [Column("api_call_count")]
    public int ApiCallCount { get; set; } = 0;

    [Column("purchase_count")]
    public int PurchaseCount { get; set; } = 0;

    [Column("revenue", TypeName = "decimal(10,2)")]
    public decimal Revenue { get; set; } = 0.00m;

    [Column("avg_execution_time")]
    public int? AvgExecutionTime { get; set; }

    [Column("error_count")]
    public int ErrorCount { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("ToolId")]
    public Tool? Tool { get; set; }
}

