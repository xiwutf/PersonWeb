using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 前端样式变量表
/// </summary>
[Table("frontend_style_variable")]
public class FrontendStyleVariable
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("page_key")]
    public string PageKey { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("variable_key")]
    public string VariableKey { get; set; } = string.Empty;

    [Required]
    [Column("variable_value", TypeName = "text")]
    public string VariableValue { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("variable_type")]
    public string VariableType { get; set; } = "color";

    [MaxLength(255)]
    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
