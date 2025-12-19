using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 项目任务表
/// </summary>
[Table("side_project_task")]
public class SideProjectTask
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("project_id")]
    public int ProjectId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [Column("status")]
    public int Status { get; set; } = 0; // 0=未开始，1=进行中，2=已完成，3=已取消

    [Column("priority")]
    public int? Priority { get; set; } // 优先级：0=低，1=中，2=高，3=紧急

    [Column("due_at", TypeName = "datetime")]
    public DateTime? DueAt { get; set; } // 截止时间

    [Column("est_hours")]
    public decimal? EstHours { get; set; } // 预计工时（小时）

    [Column("act_hours")]
    public decimal? ActHours { get; set; } // 实际工时（小时）

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0; // 排序顺序

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("ProjectId")]
    public virtual SideProject? Project { get; set; }
}

