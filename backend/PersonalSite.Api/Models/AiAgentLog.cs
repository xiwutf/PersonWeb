using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// AI 智能体调用日志表
/// 用于记录所有 AI 调用的请求和响应，便于监控和调试
/// </summary>
[Table("ai_agent_log")]
public class AiAgentLog
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// 智能体类型：Content（内容生成）、Demo（Demo上架）、Lead（线索处理）
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("agent_type")]
    public string AgentType { get; set; } = string.Empty;

    /// <summary>
    /// 请求内容（JSON 格式）
    /// </summary>
    [Column("request_payload", TypeName = "text")]
    public string? RequestPayload { get; set; }

    /// <summary>
    /// 响应内容（JSON 格式）
    /// </summary>
    [Column("response_payload", TypeName = "text")]
    public string? ResponsePayload { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    [Column("success")]
    public bool Success { get; set; }

    /// <summary>
    /// 错误信息（如果失败）
    /// </summary>
    [MaxLength(1000)]
    [Column("error_message")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

