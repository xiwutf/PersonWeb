using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 文章表
/// </summary>
[Table("article")]
public class Article
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(255)]
    [Column("slug")]
    public string? Slug { get; set; }

    [MaxLength(500)]
    [Column("summary")]
    public string? Summary { get; set; }

    [Column("content_md", TypeName = "longtext")]
    public string? ContentMd { get; set; }

    [Column("content_html", TypeName = "longtext")]
    public string? ContentHtml { get; set; }

    [MaxLength(500)]
    [Column("cover_url")]
    public string? CoverUrl { get; set; }

    [Column("category_id")]
    public long? CategoryId { get; set; }

    /// <summary>
    /// 状态：0-草稿 1-已发布 2-下线
    /// </summary>
    [Column("status")]
    public sbyte Status { get; set; } = 1;

    [Column("author_id")]
    public long? AuthorId { get; set; }

    [Column("publish_time")]
    public DateTime? PublishTime { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 导航属性
    public Category? Category { get; set; }
    
    [ForeignKey("AuthorId")]
    public User? Author { get; set; }
    
    public List<Tag> Tags { get; set; } = new();
    
    // 辅助属性：浏览量（数据库中没有这个字段，可能是遗漏或者设计变更，暂时保留属性但不映射，或者如果用户之前的代码有用到，需要确认。
    // 用户给的 SQL 里没有 view_count。
    // 之前的代码有 ViewCount。
    // 既然用户给了明确的 SQL，我应该遵循 SQL。
    // 但是 ArticlesController 用到了 ViewCount。
    // 我会添加 [NotMapped] 或者暂时移除它，并在 Controller 中注释掉相关代码，或者询问用户。
    // 鉴于用户说 "修改当前代码字段"，我应该严格匹配 SQL。
    // 所以 ViewCount 应该移除。
}
