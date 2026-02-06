namespace PersonalSite.Api.Models.Dto;

/// <summary>
/// 思维记录列表项 DTO
/// </summary>
public class ThoughtListItemDto
{
    public long Id { get; set; }
    /// <summary>摘要（原文前若干字）</summary>
    public string Summary { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 思维记录详情/完整 DTO
/// </summary>
public class ThoughtDto
{
    public long Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? AiComment { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 创建思维记录请求 DTO
/// </summary>
public class ThoughtCreateDto
{
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// 更新思维记录请求 DTO
/// </summary>
public class ThoughtUpdateDto
{
    public string Content { get; set; } = string.Empty;
}
