using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 错误日志表
/// </summary>
[Table("error_log")]
public class ErrorLog
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [MaxLength(50)]
    [Column("error_type")]
    public string ErrorType { get; set; } = string.Empty; // JavaScript, API, Server

    [MaxLength(500)]
    [Column("error_message")]
    public string ErrorMessage { get; set; } = string.Empty;

    [Column("error_stack", TypeName = "text")]
    public string? ErrorStack { get; set; }

    [MaxLength(500)]
    [Column("error_url")]
    public string? ErrorUrl { get; set; }

    [MaxLength(45)]
    [Column("user_ip")]
    public string? UserIp { get; set; }

    [MaxLength(500)]
    [Column("user_agent")]
    public string? UserAgent { get; set; }

    [MaxLength(36)]
    [Column("visitor_id")]
    public string? VisitorId { get; set; }

    [Column("metadata", TypeName = "text")]
    public string? Metadata { get; set; } // JSON 格式的额外信息

    [Column("status")]
    public sbyte Status { get; set; } = 0; // 0-未处理 1-已处理 2-已忽略

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("resolved_at")]
    public DateTime? ResolvedAt { get; set; }
}

