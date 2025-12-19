using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 项目需求表
/// </summary>
[Table("side_project_requirement")]
public class SideProjectRequirement
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("project_id")]
    public int ProjectId { get; set; }

    [Column("scope_in", TypeName = "text")]
    public string? ScopeIn { get; set; } // 范围内需求

    [Column("scope_out", TypeName = "text")]
    public string? ScopeOut { get; set; } // 范围外需求

    [Column("acceptance_criteria", TypeName = "text")]
    public string? AcceptanceCriteria { get; set; } // 验收标准

    [Column("deliverables", TypeName = "text")]
    public string? Deliverables { get; set; } // 交付物（JSON格式或文本）

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("ProjectId")]
    public virtual SideProject? Project { get; set; }
}

