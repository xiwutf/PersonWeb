using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 未来规划表
/// </summary>
[Table("roadmap")]
public class Roadmap
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [Column("items", TypeName = "text")]
    public string? Items { get; set; } // JSON 数组字符串

    [Required]
    [MaxLength(50)]
    [Column("timeline")]
    public string Timeline { get; set; } = "short"; // short, medium, long

    [Column("priority")]
    public int Priority { get; set; } = 0; // 0-低, 1-中, 2-高, 3-紧急

    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = "planned"; // planned, in_progress, completed, cancelled

    [Column("start_date", TypeName = "date")]
    public DateTime? StartDate { get; set; }

    [Column("target_date", TypeName = "date")]
    public DateTime? TargetDate { get; set; }

    [Column("completed_date", TypeName = "date")]
    public DateTime? CompletedDate { get; set; }

    [Column("progress")]
    public int Progress { get; set; } = 0; // 进度百分比 0-100

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

