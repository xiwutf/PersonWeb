using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrackingController : ControllerBase
{
    private readonly AppDbContext _context;

    public TrackingController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 记录站点访问
    /// </summary>
    [HttpPost("visit")]
    public async Task<ActionResult<ApiResponse<object>>> RecordVisit([FromBody] VisitRequest request)
    {
        var visitorId = request.VisitorId;
        if (string.IsNullOrEmpty(visitorId))
        {
            visitorId = Guid.NewGuid().ToString();
        }

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers["User-Agent"].ToString();

        var log = new VisitLog
        {
            VisitorId = visitorId,
            Ip = ip,
            UserAgent = userAgent,
            Path = request.Path ?? "unknown",
            Timestamp = DateTime.Now
        };

        _context.VisitLogs.Add(log);
        await _context.SaveChangesAsync();

        // 获取统计数据
        var totalVisits = await _context.VisitLogs.CountAsync();
        var todayVisits = await _context.VisitLogs
            .Where(v => v.Timestamp.Date == DateTime.Today)
            .CountAsync();

        return Ok(ApiResponse.Success(new
        {
            TotalVisits = totalVisits,
            TodayVisits = todayVisits,
            VisitorId = visitorId
        }));
    }

    /// <summary>
    /// 记录文章阅读
    /// </summary>
    [HttpPost("view")]
    public async Task<ActionResult<ApiResponse<object>>> RecordView([FromBody] ViewRequest request)
    {
        if (string.IsNullOrEmpty(request.Slug))
        {
            return BadRequest(ApiResponse.Error("Slug is required"));
        }

        var stat = await _context.PageStats.FirstOrDefaultAsync(s => s.Slug == request.Slug);
        if (stat == null)
        {
            stat = new PageStat
            {
                Slug = request.Slug,
                ViewCount = 0,
                UpdatedAt = DateTime.Now
            };
            _context.PageStats.Add(stat);
        }

        stat.ViewCount++;
        stat.UpdatedAt = DateTime.Now;
        
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success(new { Views = stat.ViewCount }));
    }
}

public class VisitRequest
{
    public string? VisitorId { get; set; }
    public string? Path { get; set; }
}

public class ViewRequest
{
    public string Slug { get; set; } = string.Empty;
}
