using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 工具使用记录表
/// </summary>
[Table("tool_usage")]
public class ToolUsage
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("tool_id")]
    public long ToolId { get; set; }

    [MaxLength(100)]
    [Column("user_id")]
    public string? UserId { get; set; }

    [MaxLength(50)]
    [Column("usage_type")]
    public string UsageType { get; set; } = "web";

    [Column("request_data", TypeName = "json")]
    public string? RequestData { get; set; }

    [Column("response_data", TypeName = "json")]
    public string? ResponseData { get; set; }

    [Column("execution_time")]
    public int? ExecutionTime { get; set; }

    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "success";

    [Column("error_message", TypeName = "text")]
    public string? ErrorMessage { get; set; }

    [MaxLength(50)]
    [Column("ip_address")]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    [Column("user_agent")]
    public string? UserAgent { get; set; }

    [MaxLength(500)]
    [Column("referrer")]
    public string? Referrer { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("ToolId")]
    public Tool? Tool { get; set; }
}

