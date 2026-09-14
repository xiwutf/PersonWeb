namespace PersonalSite.Api.Models.Dto;

/// <summary>
/// MindTrace 外部发布请求体
/// </summary>
public class MindtraceIngestRequest
{
    public string? Version { get; set; }
    public string? Source { get; set; }
    public string? Type { get; set; }
    public string? ExternalId { get; set; }
    public string? SelectedText { get; set; }
    public string? Note { get; set; }
    public string? PageTitle { get; set; }
    public string? PageUrl { get; set; }
    public string? CreatedAt { get; set; }
    public string? UpdatedAt { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? Keywords { get; set; }
}

/// <summary>
/// 阅读条目对外 DTO（admin / ingest 响应）
/// </summary>
public class ReadingEntryDto
{
    public long Id { get; set; }
    public string SourceType { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public string Quote { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string SourceTitle { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public DateTime? SourceCreatedAt { get; set; }
    public DateTime ImportedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
    public string Status { get; set; } = "inbox";
    public string Visibility { get; set; } = "private";
}

/// <summary>
/// 管理端状态变更请求
/// </summary>
public class ReadingActionRequest
{
    public string? Action { get; set; }
}

/// <summary>
/// MindTrace ingest 配置
/// </summary>
public class MindtraceIngestOptions
{
    public const string SectionName = "MindTrace";

    /// <summary>Bearer Token；未配置时拒绝 ingest</summary>
    public string? IngestToken { get; set; }
}
