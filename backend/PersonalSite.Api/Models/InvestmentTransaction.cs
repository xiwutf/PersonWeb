using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 投资交易记录表
/// </summary>
[Table("investment_transaction")]
public class InvestmentTransaction
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("investment_id")]
    public long InvestmentId { get; set; }

    [MaxLength(20)]
    [Column("transaction_type")]
    public string TransactionType { get; set; } = string.Empty; // buy / sell

    [Column("quantity")]
    public decimal Quantity { get; set; } = 0; // 交易数量

    [Column("price")]
    public decimal Price { get; set; } = 0; // 交易价格

    [Column("amount")]
    public decimal Amount { get; set; } = 0; // 交易金额

    [Column("fee")]
    public decimal Fee { get; set; } = 0; // 手续费

    [Column("transaction_date")]
    public DateTime TransactionDate { get; set; } = DateTime.Now;

    [Column("notes", TypeName = "text")]
    public string? Notes { get; set; } // 备注

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("InvestmentId")]
    public Investment? Investment { get; set; }
}

