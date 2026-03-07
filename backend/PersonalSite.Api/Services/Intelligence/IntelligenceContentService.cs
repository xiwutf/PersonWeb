using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using System.Text.Json;

namespace PersonalSite.Api.Services;

/// <summary>情报内容服务实现</summary>
public class IntelligenceContentService : IIntelligenceContentService
{
    private readonly AppDbContext _context;

    public IntelligenceContentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(int Total, List<ContentItemDto> List)> GetPageAsync(ContentQueryDto dto, CancellationToken cancellationToken = default)
    {
        var query = _context.IntelligenceContents.AsNoTracking();

        // 关联分析结果
        query = query.Include(c => c.Analysis);

        // 关键词搜索
        if (!string.IsNullOrWhiteSpace(dto.Keyword))
        {
            var keyword = dto.Keyword.Trim();
            query = query.Where(c =>
                c.Title.Contains(keyword) ||
                c.CleanText != null && c.CleanText.Contains(keyword));
        }

        // 分类筛选
        if (!string.IsNullOrWhiteSpace(dto.Category))
        {
            query = query.Where(c => c.Analysis != null && c.Analysis.Category == dto.Category);
        }

        // 来源筛选
        if (dto.SourceId.HasValue)
        {
            query = query.Where(c => c.SourceId == dto.SourceId.Value);
        }

        // 日期范围筛选
        if (dto.StartDate.HasValue)
        {
            query = query.Where(c => c.PublishTime >= dto.StartDate.Value);
        }
        if (dto.EndDate.HasValue)
        {
            query = query.Where(c => c.PublishTime <= dto.EndDate.Value.AddDays(1).AddSeconds(-1));
        }

        // 仅看高价值
        if (dto.HighValueOnly == true)
        {
            query = query.Where(c =>
                c.Analysis != null &&
                (c.Analysis.RelevanceScore >= 70 || c.Analysis.LearningValue == "高"));
        }

        // 只查询已分析成功的内容
        query = query.Where(c => c.AnalyzeStatus == "success");

        int total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((dto.PageIndex - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToListAsync(cancellationToken);

        var list = items.Select(c => new ContentItemDto
        {
            Id = c.Id,
            SourceId = c.SourceId,
            SourceName = c.Source != null ? c.Source.SourceName : "",
            Title = c.Title,
            OriginalUrl = c.OriginalUrl,
            PublishTime = c.PublishTime,
            Summary = c.Analysis != null ? c.Analysis.Summary : null,
            Category = c.Analysis != null ? c.Analysis.Category : "",
            Tags = c.Analysis != null ? c.Analysis.Tags : new List<string>(),
            RelevanceScore = c.Analysis != null ? c.Analysis.RelevanceScore : 0,
            LearningValue = c.Analysis != null ? c.Analysis.LearningValue : "未分析",
            BusinessValue = c.Analysis != null ? c.Analysis.BusinessValue : "未分析",
            AnalyzeStatus = c.AnalyzeStatus,
            CreatedAt = c.CreatedAt
        }).ToList();

        return (total, list);
    }

    public async Task<ContentItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var content = await _context.IntelligenceContents
            .Include(c => c.Analysis)
            .Include(c => c.Source)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (content == null)
            return null;

        return new ContentItemDto
        {
            Id = content.Id,
            SourceId = content.SourceId,
            SourceName = content.Source != null ? content.Source.SourceName : "",
            Title = content.Title,
            OriginalUrl = content.OriginalUrl,
            PublishTime = content.PublishTime,
            Summary = content.Analysis?.Summary,
            Category = content.Analysis?.Category ?? "",
            Tags = content.Analysis?.Tags ?? new List<string>(),
            RelevanceScore = content.Analysis?.RelevanceScore ?? 0,
            LearningValue = content.Analysis?.LearningValue ?? "未分析",
            BusinessValue = content.Analysis?.BusinessValue ?? "未分析",
            AnalyzeStatus = content.AnalyzeStatus,
            CreatedAt = content.CreatedAt
        };
    }

    public async Task<List<CategoryStatDto>> GetCategoryStatsAsync(CancellationToken cancellationToken = default)
    {
        var stats = await _context.IntelligenceAnalyses
            .AsNoTracking()
            .GroupBy(a => a.Category)
            .Select(g => new CategoryStatDto
            {
                Category = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(s => s.Count)
            .ToListAsync(cancellationToken);

        return stats;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var todayStart = new DateTime(today.Year, today.Month, today.Day);
        var todayEnd = todayStart.AddDays(1);

        // 今日采集数量
        var todayCollected = await _context.IntelligenceContents
            .CountAsync(c => c.CreatedAt >= todayStart && c.CreatedAt < todayEnd, cancellationToken);

        // 今日高价值内容数量
        var todayHighValue = await _context.IntelligenceContents
            .CountAsync(c =>
                c.CreatedAt >= todayStart && c.CreatedAt < todayEnd &&
                c.Analysis != null &&
                (c.Analysis.RelevanceScore >= 70 || c.Analysis.LearningValue == "高"),
                cancellationToken);

        // 最新日报
        var latestReport = await _context.IntelligenceDailyReports
            .AsNoTracking()
            .OrderByDescending(r => r.ReportDate)
            .FirstOrDefaultAsync(cancellationToken);

        // 最近 7 天日报
        var sevenDaysAgo = DateTime.Today.AddDays(-7);
        var recentReports = await _context.IntelligenceDailyReports
            .AsNoTracking()
            .Where(r => r.ReportDate >= sevenDaysAgo)
            .OrderByDescending(r => r.ReportDate)
            .ToListAsync(cancellationToken);

        // 最近内容（前 10 条）
        var recentContentsQuery = _context.IntelligenceContents
            .Include(c => c.Analysis)
            .Include(c => c.Source)
            .AsNoTracking()
            .Where(c => c.AnalyzeStatus == "success")
            .OrderByDescending(c => c.CreatedAt)
            .Take(10);

        var recentContents = await recentContentsQuery
            .ToListAsync(cancellationToken);

        var recentContentsList = recentContents.Select(c => new ContentItemDto
        {
            Id = c.Id,
            SourceId = c.SourceId,
            SourceName = c.Source != null ? c.Source.SourceName : "",
            Title = c.Title,
            OriginalUrl = c.OriginalUrl,
            PublishTime = c.PublishTime,
            Summary = c.Analysis?.Summary,
            Category = c.Analysis?.Category ?? "",
            Tags = c.Analysis?.Tags ?? new List<string>(),
            RelevanceScore = c.Analysis?.RelevanceScore ?? 0,
            LearningValue = c.Analysis?.LearningValue ?? "未分析",
            BusinessValue = c.Analysis?.BusinessValue ?? "未分析",
            AnalyzeStatus = c.AnalyzeStatus,
            CreatedAt = c.CreatedAt
        }).ToList();

        // 分类统计
        var categoryStats = await GetCategoryStatsAsync(cancellationToken);

        // 最近任务状态
        var recentTaskStatus = await _context.IntelligenceTaskLogs
            .AsNoTracking()
            .OrderByDescending(t => t.StartTime)
            .FirstOrDefaultAsync(cancellationToken);

        return new DashboardStatsDto
        {
            TodayCollected = todayCollected,
            TodayHighValue = todayHighValue,
            LatestReport = latestReport != null ? new ReportResponseDto
            {
                Id = latestReport.Id,
                ReportDate = latestReport.ReportDate,
                Title = latestReport.Title,
                ContentMarkdown = latestReport.ContentMarkdown ?? "",
                ItemCount = latestReport.ItemCount,
                GeneratedAt = latestReport.GeneratedAt
            } : null,
            RecentReports = recentReports.Select(r => new ReportResponseDto
            {
                Id = r.Id,
                ReportDate = r.ReportDate,
                Title = r.Title,
                ContentMarkdown = r.ContentMarkdown ?? "",
                ItemCount = r.ItemCount,
                GeneratedAt = r.GeneratedAt
            }).ToList(),
            RecentContents = recentContentsList,
            CategoryStats = categoryStats,
            RecentTaskStatus = recentTaskStatus != null ? new TaskLogDto
            {
                Id = recentTaskStatus.Id,
                TaskName = recentTaskStatus.TaskName,
                TaskType = recentTaskStatus.TaskType,
                Status = recentTaskStatus.Status,
                StartTime = recentTaskStatus.StartTime,
                EndTime = recentTaskStatus.EndTime,
                Message = recentTaskStatus.Message,
                CreatedAt = recentTaskStatus.CreatedAt
            } : null
        };
    }
}
