using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 项目附件表
/// </summary>
[Table("side_project_attachment")]
public class SideProjectAttachment
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("project_id")]
    public int ProjectId { get; set; }

    [MaxLength(50)]
    [Column("type")]
    public string? Type { get; set; } // 附件类型：文档/图片/代码/其他

    [Required]
    [MaxLength(500)]
    [Column("name")]
    public string Name { get; set; } = string.Empty; // 附件名称

    [Required]
    [MaxLength(1000)]
    [Column("url")]
    public string Url { get; set; } = string.Empty; // 附件URL或路径

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("ProjectId")]
    public virtual SideProject? Project { get; set; }
}

