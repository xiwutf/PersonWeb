using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 关系跟进 - 互动记录表
/// </summary>
[Table("relation_interaction")]
public class RelationInteraction
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 用户ID（支持多租户，可为空以兼容单用户模式）
    /// </summary>
    [MaxLength(100)]
    [Column("user_id")]
    public string? UserId { get; set; }

    /// <summary>
    /// 关联的对象ID
    /// </summary>
    [Required]
    [Column("person_id")]
    public Guid PersonId { get; set; }

    /// <summary>
    /// 互动类型：0=文字, 1=语音, 2=通话, 3=见面, 4=其他
    /// </summary>
    [Column("type")]
    public int Type { get; set; } = 0;

    /// <summary>
    /// 发生时间
    /// </summary>
    [Required]
    [Column("occurred_at", TypeName = "datetime")]
    public DateTime OccurredAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 摘要（必填）
    /// </summary>
    [Required]
    [Column("summary", TypeName = "text")]
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// 要点/承诺/情绪（JSON 对象）
    /// </summary>
    [Column("key_points", TypeName = "text")]
    public string? KeyPoints { get; set; }

    /// <summary>
    /// 我的感受：0=正, 1=中, 2=负
    /// </summary>
    [Column("my_feeling")]
    public int? MyFeeling { get; set; }

    /// <summary>
    /// AI 建议
    /// </summary>
    [Column("ai_suggestion", TypeName = "text")]
    public string? AiSuggestion { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("PersonId")]
    public virtual RelationPerson? Person { get; set; }
}

