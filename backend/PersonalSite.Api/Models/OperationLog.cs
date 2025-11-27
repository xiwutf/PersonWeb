using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 操作日志表
/// </summary>
[Table("operation_log")]
public class OperationLog
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("user_id")]
    public long? UserId { get; set; }

    [MaxLength(100)]
    [Column("action")]
    public string? Action { get; set; }

    [MaxLength(50)]
    [Column("target_type")]
    public string? TargetType { get; set; }

    [Column("target_id")]
    public long? TargetId { get; set; }

    [Column("detail", TypeName = "text")]
    public string? Detail { get; set; }

    [MaxLength(50)]
    [Column("ip")]
    public string? Ip { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [ForeignKey("UserId")]
    public User? User { get; set; }
}
