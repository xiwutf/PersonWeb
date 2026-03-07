namespace PersonalSite.Api.Models.Dto;

/// <summary>来源创建/更新请求</summary>
public class SourceRequestDto
{
    public string SourceName { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty; // RSS | WEB
    public string SourceUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public int Priority { get; set; }
    public bool Enabled { get; set; }
    public int FetchIntervalMinutes { get; set; } = 60;
    public string? Remark { get; set; }
}

/// <summary>来源响应</summary>
public class SourceResponseDto
{
    public int Id { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public int Priority { get; set; }
    public bool Enabled { get; set; }
    public int FetchIntervalMinutes { get; set; }
    public DateTime? LastFetchTime { get; set; }
    public string? Remark { get; set; }
}

/// <summary>分类统计</summary>
public class CategoryStatDto
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
}
