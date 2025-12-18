using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 名字收藏表
/// </summary>
[Table("name_favorite")]
public class NameFavorite
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("type")]
    public string Type { get; set; } = string.Empty; // game | nickname | english

    [MaxLength(200)]
    [Column("style")]
    public string? Style { get; set; } // 多风格用逗号存储

    [MaxLength(20)]
    [Column("language")]
    public string? Language { get; set; }

    [Column("total_score")]
    public int TotalScore { get; set; }

    [MaxLength(500)]
    [Column("reason")]
    public string? Reason { get; set; }

    [Column("meta_json", TypeName = "text")]
    public string? MetaJson { get; set; } // 存储维度分、输入条件等（JSON格式）

    /// <summary>
    /// 所有者标识（预留，当前用于匿名收藏）
    /// </summary>
    [MaxLength(64)]
    [Column("owner_key")]
    public string? OwnerKey { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

