using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 项目里程碑表
/// </summary>
[Table("side_project_milestone")]
public class SideProjectMilestone
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

    [Column("due_at", TypeName = "datetime")]
    public DateTime? DueAt { get; set; } // 截止时间

    [Column("status")]
    public int Status { get; set; } = 0; // 0=未完成，1=已完成

    [Column("notes", TypeName = "text")]
    public string? Notes { get; set; } // 备注

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("ProjectId")]
    public virtual SideProject? Project { get; set; }
}

