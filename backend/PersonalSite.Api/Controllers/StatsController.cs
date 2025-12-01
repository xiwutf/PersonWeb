using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StatsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取访问统计数据
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize] // 仅管理员可访问
    public async Task<ActionResult<ApiResponse<object>>> GetStats()
    {
        // 1. 总访问量
        var totalVisits = await _context.VisitLogs.CountAsync();

        // 2. 独立访客数
        var uniqueVisitors = await _context.VisitLogs
            .Select(v => v.VisitorId)
            .Distinct()
            .CountAsync();

        // 3. 今日访问量
        var today = DateTime.Today;
        var todayVisits = await _context.VisitLogs
            .Where(v => v.Timestamp >= today)
            .CountAsync();

        // 4. 热门路径 (Top 5) - 过滤空路径
        var topPaths = await _context.VisitLogs
            .Where(v => !string.IsNullOrEmpty(v.Path))
            .GroupBy(v => v.Path)
            .Select(g => new { Path = g.Key ?? "/", Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();

        // 5. 最近访问记录 (Top 50) - 确保路径不为空
        var recentVisits = await _context.VisitLogs
            .OrderByDescending(v => v.Timestamp)
            .Take(50)
            .Select(v => new 
            {
                v.Id,
                v.VisitorId,
                v.Ip,
                Path = v.Path ?? "/",
                v.Timestamp
            })
            .ToListAsync();

        // 6. 统计数量
        var articleCount = await _context.Articles.CountAsync();
        var projectCount = await _context.Projects.CountAsync();

        // 7. 待审核留言数（时间胶囊）
        var pendingMessages = await _context.VisitorMessages
            .CountAsync(m => m.Status == "pending");

        // 8. 待办任务数
        var pendingTasks = await _context.Tasks
            .CountAsync(t => t.Status == "pending" || t.Status == "in_progress");

        // 9. 最近7天访问趋势（用于图表）
        var sevenDaysAgo = DateTime.Today.AddDays(-6);
        // 先获取分组数据，然后在内存中格式化日期（因为 EF Core 无法翻译 ToString）
        var visitTrendData = await _context.VisitLogs
            .Where(v => v.Timestamp >= sevenDaysAgo)
            .GroupBy(v => v.Timestamp.Date)
            .Select(g => new 
            { 
                Date = g.Key, 
                Count = g.Count(),
                UniqueVisitors = g.Select(v => v.VisitorId).Distinct().Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync();
        
        // 在内存中格式化日期字符串
        var visitTrend = visitTrendData.Select(g => new 
        { 
            Date = g.Date.ToString("yyyy-MM-dd"), 
            Count = g.Count,
            UniqueVisitors = g.UniqueVisitors
        }).ToList();

        // 10. 在线人数（最近5分钟）- 从 VisitorAnalytics 或 VisitLogs 获取
        int onlineCount = 0;
        try
        {
            var analyticsCount = await _context.VisitorAnalytics.CountAsync();
            if (analyticsCount > 0)
            {
                onlineCount = await _context.VisitorAnalytics
                    .Where(v => v.IsOnline && v.UpdatedAt >= DateTime.Now.AddMinutes(-5))
                    .Select(v => v.VisitorId)
                    .Distinct()
                    .CountAsync();
            }
            else
            {
                // 如果 VisitorAnalytics 为空，从 VisitLogs 获取最近5分钟的访问
                onlineCount = await _context.VisitLogs
                    .Where(v => v.Timestamp >= DateTime.Now.AddMinutes(-5))
                    .Select(v => v.VisitorId)
                    .Distinct()
                    .CountAsync();
            }
        }
        catch
        {
            // 如果查询失败，使用默认值 0
            onlineCount = 0;
        }

        // 11. 昨日访问量（用于对比）
        var yesterday = today.AddDays(-1);
        var yesterdayVisits = await _context.VisitLogs
            .Where(v => v.Timestamp >= yesterday && v.Timestamp < today)
            .CountAsync();

        return Ok(ApiResponse.Success(new 
        {
            TotalVisits = totalVisits,
            UniqueVisitors = uniqueVisitors,
            TodayVisits = todayVisits,
            YesterdayVisits = yesterdayVisits,
            OnlineCount = onlineCount,
            TopPaths = topPaths,
            RecentVisits = recentVisits,
            ArticleCount = articleCount,
            ProjectCount = projectCount,
            PendingMessages = pendingMessages,
            PendingTasks = pendingTasks,
            VisitTrend = visitTrend
        }));
    }
}
