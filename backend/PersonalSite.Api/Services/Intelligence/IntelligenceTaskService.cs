using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Services;

/// <summary>情报任务服务实现</summary>
public class IntelligenceTaskService : IIntelligenceTaskService
{
    private readonly AppDbContext _context;

    public IntelligenceTaskService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskTriggerResponseDto> TriggerCollectAsync(CancellationToken cancellationToken = default)
    {
        var taskLog = new IntelligenceTaskLog
        {
            TaskName = "collect",
            Status = "running",
            StartTime = DateTime.Now
        };

        _context.IntelligenceTaskLogs.Add(taskLog);
        await _context.SaveChangesAsync(cancellationToken);

        return new TaskTriggerResponseDto
        {
            TaskId = taskLog.Id.ToString(),
            Message = "采集任务已提交"
        };
    }

    public async Task<TaskTriggerResponseDto> TriggerAnalyzeAsync(CancellationToken cancellationToken = default)
    {
        var taskLog = new IntelligenceTaskLog
        {
            TaskName = "analyze",
            Status = "running",
            StartTime = DateTime.Now
        };

        _context.IntelligenceTaskLogs.Add(taskLog);
        await _context.SaveChangesAsync(cancellationToken);

        return new TaskTriggerResponseDto
        {
            TaskId = taskLog.Id.ToString(),
            Message = "分析任务已提交"
        };
    }

    public async Task<TaskTriggerResponseDto> TriggerGenerateReportAsync(CancellationToken cancellationToken = default)
    {
        var taskLog = new IntelligenceTaskLog
        {
            TaskName = "generate_report",
            Status = "running",
            StartTime = DateTime.Now
        };

        _context.IntelligenceTaskLogs.Add(taskLog);
        await _context.SaveChangesAsync(cancellationToken);

        return new TaskTriggerResponseDto
        {
            TaskId = taskLog.Id.ToString(),
            Message = "日报生成任务已提交"
        };
    }

    public async Task<(int Total, List<TaskLogDto> List)> GetLogsAsync(int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = _context.IntelligenceTaskLogs.AsNoTracking();
        int total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.StartTime)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var list = items.Select(t => new TaskLogDto
        {
            Id = t.Id,
            TaskName = t.TaskName,
            TaskType = t.TaskType,
            Status = t.Status,
            StartTime = t.StartTime,
            EndTime = t.EndTime,
            Message = t.Message,
            CreatedAt = t.CreatedAt
        }).ToList();

        return (total, list);
    }
}
