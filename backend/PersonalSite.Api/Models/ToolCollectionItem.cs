using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 工具合集关联表
/// </summary>
[Table("tool_collection_item")]
public class ToolCollectionItem
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("collection_id")]
    public long CollectionId { get; set; }

    [Column("tool_id")]
    public long ToolId { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("CollectionId")]
    public ToolCollection? Collection { get; set; }

    [ForeignKey("ToolId")]
    public Tool? Tool { get; set; }
}

