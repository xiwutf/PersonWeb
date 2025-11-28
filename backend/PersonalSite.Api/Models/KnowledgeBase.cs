using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 个人知识库表
/// </summary>
[Table("knowledge_base")]
public class KnowledgeBase
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("content", TypeName = "longtext")]
    public string? Content { get; set; }

    [MaxLength(50)]
    [Column("category")]
    public string? Category { get; set; } // 开发笔记 / 踩坑记录 / 想法灵感

    [MaxLength(500)]
    [Column("tags")]
    public string? Tags { get; set; } // JSON 数组或逗号分隔

    [Column("version")]
    public int Version { get; set; } = 1;

    [Column("parent_id")]
    public long? ParentId { get; set; } // 版本历史关联

    [Column("status")]
    public sbyte Status { get; set; } = 1; // 0-草稿 1-已发布 2-已归档

    [Column("view_count")]
    public int ViewCount { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [Column("author_id")]
    public long? AuthorId { get; set; }

    // 导航属性
    [ForeignKey("AuthorId")]
    public User? Author { get; set; }
}

