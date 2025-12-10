using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 个人助理会话表
/// </summary>
[Table("assistant_sessions")]
public class AssistantSession
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// 用户 ID（管理员）
    /// </summary>
    [Column("user_id")]
    public long UserId { get; set; }

    /// <summary>
    /// 会话标题（可自动生成）
    /// </summary>
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    public List<AssistantMessage> Messages { get; set; } = new();
}

/// <summary>
/// 个人助理消息表
/// </summary>
[Table("assistant_messages")]
public class AssistantMessage
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// 会话 ID
    /// </summary>
    [Column("session_id")]
    public long SessionId { get; set; }

    /// <summary>
    /// 角色：User / Assistant
    /// </summary>
    [Required]
    [MaxLength(20)]
    [Column("role")]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    [Column("content", TypeName = "text")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("SessionId")]
    public AssistantSession? Session { get; set; }
}

