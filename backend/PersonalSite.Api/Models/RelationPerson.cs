using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 关系跟进 - 对象表
/// </summary>
[Table("relation_person")]
public class RelationPerson
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
    /// 昵称（必填）
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column("nickname")]
    public string Nickname { get; set; } = string.Empty;

    /// <summary>
    /// 来源（如：朋友介绍/社交软件/活动等）
    /// </summary>
    [MaxLength(200)]
    [Column("source")]
    public string? Source { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [MaxLength(100)]
    [Column("city")]
    public string? City { get; set; }

    /// <summary>
    /// 标签（JSON 数组字符串，如：["程序员","喜欢读书","有趣"]）
    /// </summary>
    [Column("tags", TypeName = "text")]
    public string? Tags { get; set; }

    /// <summary>
    /// 喜好/雷点/关键点
    /// </summary>
    [Column("preferences", TypeName = "text")]
    public string? Preferences { get; set; }

    /// <summary>
    /// 阶段：0=新认识, 1=熟悉中, 2=准备约见, 3=已见面, 4=升温中, 5=观察期, 6=已结束
    /// </summary>
    [Column("stage")]
    public int Stage { get; set; } = 0;

    /// <summary>
    /// 热度分数（0-100）
    /// </summary>
    [Column("heat_score")]
    public int HeatScore { get; set; } = 0;

    /// <summary>
    /// 最后联系时间
    /// </summary>
    [Column("last_contact_at", TypeName = "datetime")]
    public DateTime? LastContactAt { get; set; }

    /// <summary>
    /// 最后见面时间
    /// </summary>
    [Column("last_meet_at", TypeName = "datetime")]
    public DateTime? LastMeetAt { get; set; }

    /// <summary>
    /// 下一步行动
    /// </summary>
    [MaxLength(500)]
    [Column("next_action")]
    public string? NextAction { get; set; }

    /// <summary>
    /// 提醒时间
    /// </summary>
    [Column("remind_at", TypeName = "datetime")]
    public DateTime? RemindAt { get; set; }

    /// <summary>
    /// 观察期开始时间
    /// </summary>
    [Column("observation_started_at", TypeName = "datetime")]
    public DateTime? ObservationStartedAt { get; set; }

    /// <summary>
    /// 观察期预计结束时间（默认开始后7天）
    /// </summary>
    [Column("observation_expected_end_at", TypeName = "datetime")]
    public DateTime? ObservationExpectedEndAt { get; set; }

    /// <summary>
    /// 观察期上次提醒时间
    /// </summary>
    [Column("observation_last_reminded_at", TypeName = "datetime")]
    public DateTime? ObservationLastRemindedAt { get; set; }

    /// <summary>
    /// 进入观察期的原因
    /// </summary>
    [MaxLength(500)]
    [Column("observation_reason")]
    public string? ObservationReason { get; set; }

    /// <summary>
    /// 是否等待观察期结束决策
    /// </summary>
    [Column("observation_decision_pending")]
    public bool ObservationDecisionPending { get; set; } = false;

    /// <summary>
    /// 备注
    /// </summary>
    [Column("notes", TypeName = "text")]
    public string? Notes { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    public virtual ICollection<RelationInteraction> Interactions { get; set; } = new List<RelationInteraction>();
    public virtual ICollection<RelationTask> Tasks { get; set; } = new List<RelationTask>();
}

