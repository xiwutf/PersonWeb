namespace PersonalSite.Api.Models.Dto;

/// <summary>内容查询参数</summary>
public class ContentQueryDto
{
    public string? Keyword { get; set; }
    public string? Category { get; set; }
    public int? SourceId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? HighValueOnly { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>内容列表响应</summary>
public class ContentListResponseDto
{
    public int Total { get; set; }
    public List<ContentItemDto> List { get; set; } = new();
}

/// <summary>内容项</summary>
public class ContentItemDto
{
    public int Id { get; set; }
    public int SourceId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public DateTime? PublishTime { get; set; }
    public string? Summary { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public int RelevanceScore { get; set; }
    public string LearningValue { get; set; } = string.Empty;
    public string BusinessValue { get; set; } = string.Empty;
    public string AnalyzeStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>仪表盘统计数据</summary>
public class DashboardStatsDto
{
    public int TodayCollected { get; set; }
    public int TodayHighValue { get; set; }
    public ReportResponseDto? LatestReport { get; set; }
    public List<ReportResponseDto> RecentReports { get; set; } = new();
    public List<ContentItemDto> RecentContents { get; set; } = new();
    public List<CategoryStatDto> CategoryStats { get; set; } = new();
    public TaskLogDto? RecentTaskStatus { get; set; }
}
