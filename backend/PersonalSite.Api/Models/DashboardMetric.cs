using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 仪表盘指标表
/// </summary>
[Table("dashboard_metric")]
public class DashboardMetric
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("date", TypeName = "date")]
    public DateTime Date { get; set; }

    [Column("steps")]
    public int Steps { get; set; } = 0;

    [Column("sleep", TypeName = "decimal(4,1)")]
    public decimal Sleep { get; set; } = 0.0m;

    [Column("weight", TypeName = "decimal(5,2)")]
    public decimal? Weight { get; set; }

    [Column("net_worth", TypeName = "decimal(12,2)")]
    public decimal? NetWorth { get; set; }

    [Column("energy")]
    public int Energy { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

