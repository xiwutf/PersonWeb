using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 工具API密钥表
/// </summary>
[Table("tool_api_key")]
public class ToolApiKey
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("tool_id")]
    public long ToolId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("api_key")]
    public string ApiKey { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("key_name")]
    public string? KeyName { get; set; }

    [Column("rate_limit")]
    public int RateLimit { get; set; } = 1000;

    [Column("usage_count")]
    public int UsageCount { get; set; } = 0;

    [Column("last_used_at")]
    public DateTime? LastUsedAt { get; set; }

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("ToolId")]
    public Tool? Tool { get; set; }
}

