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
    public string? Category { get; set; } // 网站/小程序/AI/其他

    [MaxLength(200)]
    [Column("tech_stack")]
    public string? TechStack { get; set; } // 以逗号分隔：Vue,.NET,Python

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

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

