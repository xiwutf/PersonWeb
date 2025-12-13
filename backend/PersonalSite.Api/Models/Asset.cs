using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 资产记录表（银行卡、存单等非投资类资产）
/// </summary>
[Table("asset")]
public class Asset
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("name")]
    public string Name { get; set; } = string.Empty; // 资产名称（如：招商银行储蓄卡、定期存单等）

    [Required]
    [MaxLength(50)]
    [Column("asset_type")]
    public string AssetType { get; set; } = "bank_card"; // bank_card / deposit / cash / other

    [MaxLength(100)]
    [Column("institution")]
    public string? Institution { get; set; } // 机构名称（如：招商银行、工商银行等）

    [Column("amount")]
    public decimal Amount { get; set; } = 0; // 资产金额

    [MaxLength(50)]
    [Column("currency")]
    public string Currency { get; set; } = "CNY"; // 货币类型（CNY/USD等）

    [Column("interest_rate")]
    public decimal InterestRate { get; set; } = 0; // 利率（年化，百分比）

    [Column("maturity_date")]
    public DateTime? MaturityDate { get; set; } // 到期日期（存单等）

    [Column("notes", TypeName = "text")]
    public string? Notes { get; set; } // 备注

    [Column("is_active")]
    public bool IsActive { get; set; } = true; // 是否启用

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
