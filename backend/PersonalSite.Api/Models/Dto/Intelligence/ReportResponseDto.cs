namespace PersonalSite.Api.Models.Dto;

/// <summary>日报响应</summary>
public class ReportResponseDto
{
    public int Id { get; set; }
    public DateTime ReportDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentMarkdown { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public DateTime? GeneratedAt { get; set; }
}

/// <summary>任务日志响应</summary>
public class TaskLogDto
{
    public int Id { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string? TaskType { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>任务触发响应</summary>
public class TaskTriggerResponseDto
{
    public string TaskId { get; set; } = string.Empty;
    public string Message { get; set; } = "任务已提交";
}
