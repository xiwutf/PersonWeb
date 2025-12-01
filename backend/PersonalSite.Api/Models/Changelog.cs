using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 更新日志表
/// </summary>
[Table("changelog")]
public class Changelog
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [MaxLength(50)]
    [Column("version")]
    public string? Version { get; set; }

    [Column("date", TypeName = "date")]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [Column("items", TypeName = "text")]
    public string? Items { get; set; } // JSON 数组字符串

    [MaxLength(50)]
    [Column("category")]
    public string Category { get; set; } = "feature"; // feature, fix, improvement

    [Column("status")]
    public sbyte Status { get; set; } = 1; // 0-隐藏 1-显示

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

