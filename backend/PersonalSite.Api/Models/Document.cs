using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Models;

/// <summary>
/// 文档表
/// </summary>
[Table("Document")]
public class Document
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(500)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    [Column("file_name")]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    [Column("file_path")]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("file_type")]
    public string FileType { get; set; } = string.Empty;

    [Column("file_size")]
    public long FileSize { get; set; } = 0;

    [Required]
    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = "pending"; // pending, processing, completed, failed

    [Column("summary", TypeName = "text")]
    public string? Summary { get; set; }

    [Column("knowledge_structure", TypeName = "json")]
    public string? KnowledgeStructure { get; set; } // JSON 格式

    [Column("total_chunks")]
    public int TotalChunks { get; set; } = 0;

    [MaxLength(100)]
    [Column("user_id")]
    public string? UserId { get; set; }

    [Column("error_message", TypeName = "text")]
    public string? ErrorMessage { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [Column("processed_at")]
    public DateTime? ProcessedAt { get; set; }

    // 导航属性
    public virtual ICollection<DocumentChunk> Chunks { get; set; } = new List<DocumentChunk>();
    public virtual ICollection<DocumentQuery> Queries { get; set; } = new List<DocumentQuery>();
}

/// <summary>
/// 文档分段表
/// </summary>
[Table("DocumentChunk")]
public class DocumentChunk
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("document_id")]
    public long DocumentId { get; set; }

    [Column("chunk_index")]
    public int ChunkIndex { get; set; }

    [Required]
    [Column("content", TypeName = "text")]
    public string Content { get; set; } = string.Empty;

    [Column("summary", TypeName = "text")]
    public string? Summary { get; set; }

    [Column("metadata", TypeName = "json")]
    public string? Metadata { get; set; } // JSON 格式

    [MaxLength(200)]
    [Column("vector_id")]
    public string? VectorId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("DocumentId")]
    public virtual Document? Document { get; set; }
}

/// <summary>
/// 问答历史表
/// </summary>
[Table("DocumentQuery")]
public class DocumentQuery
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("document_id")]
    public long? DocumentId { get; set; }

    [MaxLength(100)]
    [Column("user_id")]
    public string? UserId { get; set; }

    [Required]
    [Column("query", TypeName = "text")]
    public string Query { get; set; } = string.Empty;

    [Required]
    [Column("answer", TypeName = "text")]
    public string Answer { get; set; } = string.Empty;

    [Column("relevant_chunks", TypeName = "json")]
    public string? RelevantChunks { get; set; } // JSON 数组，包含 chunk_id 列表

    [Column("confidence", TypeName = "decimal(5,4)")]
    public decimal? Confidence { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("DocumentId")]
    public virtual Document? Document { get; set; }
}

