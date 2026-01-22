using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 认知说明书表
/// </summary>
[Table("cognition_doc")]
public class CognitionDoc
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("slug")]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column("summary")]
    public string? Summary { get; set; }

    [Required]
    [Column("content_md", TypeName = "longtext")]
    public string ContentMd { get; set; } = string.Empty;

    /// <summary>
    /// 状态：draft-草稿, published-已发布
    /// </summary>
    [Required]
    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "draft";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
