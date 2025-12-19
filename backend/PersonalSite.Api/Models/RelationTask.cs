using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 关系跟进 - 任务表
/// </summary>
[Table("relation_task")]
public class RelationTask
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 关联的对象ID
    /// </summary>
    [Required]
    [Column("person_id")]
    public Guid PersonId { get; set; }

    /// <summary>
    /// 任务标题
    /// </summary>
    [Required]
    [MaxLength(500)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 截止时间
    /// </summary>
    [Column("due_at", TypeName = "datetime")]
    public DateTime? DueAt { get; set; }

    /// <summary>
    /// 优先级：0=低, 1=中, 2=高, 3=紧急
    /// </summary>
    [Column("priority")]
    public int Priority { get; set; } = 1;

    /// <summary>
    /// 状态：0=todo, 1=done, 2=hold
    /// </summary>
    [Column("status")]
    public int Status { get; set; } = 0;

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("PersonId")]
    public virtual RelationPerson? Person { get; set; }
}

