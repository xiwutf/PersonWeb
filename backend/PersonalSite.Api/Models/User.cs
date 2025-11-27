using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 用户表
/// </summary>
[Table("user")]
public class User
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("email")]
    public string? Email { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("role")]
    public string Role { get; set; } = "admin";

    /// <summary>
    /// 状态：0-禁用 1-启用
    /// </summary>
    [Column("status")]
    public sbyte Status { get; set; } = 1;

    [Column("last_login_time")]
    public DateTime? LastLoginTime { get; set; }

    [MaxLength(50)]
    [Column("last_login_ip")]
    public string? LastLoginIp { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
