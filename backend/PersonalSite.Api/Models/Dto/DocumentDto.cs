namespace PersonalSite.Api.Models.Dto;

/// <summary>
/// 文档上传请求 DTO
/// </summary>
public class DocumentUploadRequest
{
    /// <summary>
    /// 文档标题（可选，如果不提供则使用文件名）
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 用户 ID
    /// </summary>
    public string? UserId { get; set; }
}

/// <summary>
/// 文档列表响应 DTO
/// </summary>
public class DocumentListItemDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public int TotalChunks { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

/// <summary>
/// 文档详情响应 DTO
/// </summary>
public class DocumentDetailDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? KnowledgeStructure { get; set; }
    public int TotalChunks { get; set; }
    public string? UserId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

/// <summary>
/// 文档分段列表响应 DTO
/// </summary>
public class DocumentChunkDto
{
    public long Id { get; set; }
    public int ChunkIndex { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 文档问答请求 DTO
/// </summary>
public class DocumentQueryRequest
{
    /// <summary>
    /// 用户问题
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// 用户 ID
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 返回相关文档片段数量（默认 5）
    /// </summary>
    public int TopK { get; set; } = 5;
}

/// <summary>
/// 文档问答响应 DTO
/// </summary>
public class DocumentQueryResponseDto
{
    /// <summary>
    /// AI 生成的答案
    /// </summary>
    public string Answer { get; set; } = string.Empty;

    /// <summary>
    /// 相关文档片段列表
    /// </summary>
    public List<RelevantChunkDto> RelevantChunks { get; set; } = new();

    /// <summary>
    /// 置信度（0-1）
    /// </summary>
    public decimal? Confidence { get; set; }
}

/// <summary>
/// 相关文档片段 DTO
/// </summary>
public class RelevantChunkDto
{
    public long ChunkId { get; set; }
    public int ChunkIndex { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public decimal Score { get; set; } // 相似度分数
}

