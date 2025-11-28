using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// API 用户表
/// </summary>
[Table("api_users")]
public class ApiUser
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("name")]
    public string? Name { get; set; }

    [MaxLength(50)]
    [Column("plan_type")]
    public string PlanType { get; set; } = "free";

    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "active";

    [Column("total_calls")]
    public long TotalCalls { get; set; } = 0;

    [Column("free_calls_used")]
    public long FreeCallsUsed { get; set; } = 0;

    [Column("paid_calls")]
    public long PaidCalls { get; set; } = 0;

    [Column("last_call_at")]
    public DateTime? LastCallAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    public List<ApiKey> ApiKeys { get; set; } = new();
}

/// <summary>
/// API Key 表
/// </summary>
[Table("api_keys")]
public class ApiKey
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("api_key")]
    public string ApiKeyValue { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("api_secret")]
    public string ApiSecret { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("name")]
    public string? Name { get; set; }

    [Column("rate_limit")]
    public int RateLimit { get; set; } = 100;

    [Column("daily_limit")]
    public int DailyLimit { get; set; } = 10000;

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("last_used_at")]
    public DateTime? LastUsedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("UserId")]
    public ApiUser? User { get; set; }
}

/// <summary>
/// API 调用记录表
/// </summary>
[Table("api_calls")]
public class ApiCall
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("api_key_id")]
    public long ApiKeyId { get; set; }

    [Column("user_id")]
    public long? UserId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("endpoint")]
    public string Endpoint { get; set; } = string.Empty;

    [MaxLength(10)]
    [Column("method")]
    public string Method { get; set; } = "POST";

    [Column("status_code")]
    public int? StatusCode { get; set; }

    [Column("response_time")]
    public int? ResponseTime { get; set; }

    [Column("request_size")]
    public int? RequestSize { get; set; }

    [Column("response_size")]
    public int? ResponseSize { get; set; }

    [Column("cost", TypeName = "decimal(10,6)")]
    public decimal Cost { get; set; } = 0;

    [MaxLength(50)]
    [Column("ip_address")]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    [Column("user_agent")]
    public string? UserAgent { get; set; }

    [Column("called_at")]
    public DateTime CalledAt { get; set; } = DateTime.Now;
}

/// <summary>
/// API 计费记录表
/// </summary>
[Table("api_billing")]
public class ApiBilling
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("period")]
    public string Period { get; set; } = string.Empty; // 格式：2024-12

    [Column("total_calls")]
    public long TotalCalls { get; set; } = 0;

    [Column("free_calls")]
    public long FreeCalls { get; set; } = 0;

    [Column("paid_calls")]
    public long PaidCalls { get; set; } = 0;

    [Column("amount", TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; } = 0.00m;

    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "pending";

    [Column("paid_at")]
    public DateTime? PaidAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

