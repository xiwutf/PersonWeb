using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Services;

/// <summary>情报任务服务接口</summary>
public interface IIntelligenceTaskService
{
    Task<TaskTriggerResponseDto> TriggerCollectAsync(CancellationToken cancellationToken = default);
    Task<TaskTriggerResponseDto> TriggerAnalyzeAsync(CancellationToken cancellationToken = default);
    Task<TaskTriggerResponseDto> TriggerGenerateReportAsync(CancellationToken cancellationToken = default);
    Task<(int Total, List<TaskLogDto> List)> GetLogsAsync(int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}
