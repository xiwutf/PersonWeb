using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 客服智能体配置表
/// 用于存储客服智能体的系统提示词和 FAQ 配置
/// </summary>
[Table("support_config")]
public class SupportConfig
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// 配置键（例如：system_prompt, faq_list）
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column("config_key")]
    public string ConfigKey { get; set; } = string.Empty;

    /// <summary>
    /// 配置值（JSON 或文本）
    /// </summary>
    [Column("config_value", TypeName = "text")]
    public string? ConfigValue { get; set; }

    /// <summary>
    /// 配置描述
    /// </summary>
    [MaxLength(500)]
    [Column("description")]
    public string? Description { get; set; }

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
}

/// <summary>
/// FAQ 条目
/// </summary>
public class FaqItem
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string? Category { get; set; }
}

