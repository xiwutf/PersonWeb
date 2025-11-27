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

        // 4. 热门路径 (Top 5)
        var topPaths = await _context.VisitLogs
            .GroupBy(v => v.Path)
            .Select(g => new { Path = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();

        // 5. 最近访问记录 (Top 50)
        var recentVisits = await _context.VisitLogs
            .OrderByDescending(v => v.Timestamp)
            .Take(50)
            .Select(v => new 
            {
                v.Id,
                v.VisitorId,
                v.Ip,
                v.Path,
                v.Timestamp
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(new 
        {
            TotalVisits = totalVisits,
            UniqueVisitors = uniqueVisitors,
            TodayVisits = todayVisits,
            TopPaths = topPaths,
            RecentVisits = recentVisits
        }));
    }
}
