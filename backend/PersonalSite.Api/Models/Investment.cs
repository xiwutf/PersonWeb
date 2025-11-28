using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 投资记录表
/// </summary>
[Table("investment")]
public class Investment
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
    public string Type { get; set; } = "stock"; // stock / fund

    [Column("quantity")]
    public decimal Quantity { get; set; } = 0; // 持仓数量

    [Column("cost_price")]
    public decimal CostPrice { get; set; } = 0; // 成本价

    [Column("current_price")]
    public decimal CurrentPrice { get; set; } = 0; // 当前价

    [Column("total_cost")]
    public decimal TotalCost { get; set; } = 0; // 总成本

    [Column("market_value")]
    public decimal MarketValue { get; set; } = 0; // 市值

    [Column("profit_loss")]
    public decimal ProfitLoss { get; set; } = 0; // 盈亏

    [Column("profit_rate")]
    public decimal ProfitRate { get; set; } = 0; // 盈亏比例

    [Column("notes", TypeName = "text")]
    public string? Notes { get; set; } // 备注

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    public List<InvestmentTransaction> Transactions { get; set; } = new();
}

