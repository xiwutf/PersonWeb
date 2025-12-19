using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 项目沟通记录表
/// </summary>
[Table("side_project_log")]
public class SideProjectLog
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("project_id")]
    public int ProjectId { get; set; }

    [MaxLength(50)]
    [Column("channel")]
    public string? Channel { get; set; } // 沟通渠道：微信/邮件/电话/会议/其他

    [Column("content", TypeName = "text")]
    public string? Content { get; set; } // 沟通内容

    [MaxLength(500)]
    [Column("next_todo")]
    public string? NextTodo { get; set; } // 下一步待办

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("ProjectId")]
    public virtual SideProject? Project { get; set; }
}

