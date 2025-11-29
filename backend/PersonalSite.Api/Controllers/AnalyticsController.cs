using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Net;
using System.Text.Json;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AnalyticsController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// 记录访问（增强版）
    /// </summary>
    [HttpPost("track")]
    public async Task<ActionResult<ApiResponse<object>>> Track([FromBody] TrackRequest? request)
    {
        try
        {
            if (request == null || string.IsNullOrEmpty(request.VisitorId))
            {
                return BadRequest(ApiResponse.Error("VisitorId is required", 400));
            }

            var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();
            var referrer = Request.Headers["Referer"].ToString();

            // 解析设备信息
            var deviceInfo = ParseDeviceInfo(userAgent);

            // 查找或创建访客会话
            var analytics = await _context.VisitorAnalytics
                .Where(v => v.VisitorId == request.VisitorId && v.IsOnline)
                .OrderByDescending(v => v.UpdatedAt)
                .FirstOrDefaultAsync();

            // 如果是新会话或IP发生变化，解析地理位置
            if (analytics == null || analytics.Ip != ip)
            {
                var geoInfo = await GetIpGeoLocation(ip);
                
                if (analytics == null)
                {
                    analytics = new VisitorAnalytics
                    {
                        VisitorId = request.VisitorId,
                        Ip = ip,
                        Country = geoInfo.Country,
                        Region = geoInfo.Region,
                        City = geoInfo.City,
                        Referrer = referrer,
                        SearchKeyword = request.SearchKeyword,
                        DeviceType = deviceInfo.DeviceType,
                        Browser = deviceInfo.Browser,
                        Os = deviceInfo.Os,
                        Path = request.Path,
                        SessionStart = DateTime.Now,
                        IsOnline = true
                    };
                    _context.VisitorAnalytics.Add(analytics);
                }
                else
                {
                    // IP变化，更新地理位置信息
                    analytics.Ip = ip;
                    analytics.Country = geoInfo.Country;
                    analytics.Region = geoInfo.Region;
                    analytics.City = geoInfo.City;
                }
            }
            else
            {
                analytics.PageViews++;
                analytics.UpdatedAt = DateTime.Now;
                analytics.IsOnline = true;
                if (analytics.Path != request.Path)
                {
                    analytics.Path = request.Path;
                }
                // 更新搜索关键词（如果有）
                if (!string.IsNullOrEmpty(request.SearchKeyword))
                {
                    analytics.SearchKeyword = request.SearchKeyword;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new { SessionId = analytics.Id }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"Track failed: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取统计数据（管理员）
    /// </summary>
    [HttpGet("stats")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetStats()
    {
        var today = DateTime.Today;
        var yesterday = today.AddDays(-1);

        // 今日访问量
        var todayPv = await _context.VisitorAnalytics
            .Where(v => v.SessionStart >= today)
            .SumAsync(v => v.PageViews);

        var todayUv = await _context.VisitorAnalytics
            .Where(v => v.SessionStart >= today)
            .Select(v => v.VisitorId)
            .Distinct()
            .CountAsync();

        // 昨日访问量
        var yesterdayPv = await _context.VisitorAnalytics
            .Where(v => v.SessionStart >= yesterday && v.SessionStart < today)
            .SumAsync(v => v.PageViews);

        var yesterdayUv = await _context.VisitorAnalytics
            .Where(v => v.SessionStart >= yesterday && v.SessionStart < today)
            .Select(v => v.VisitorId)
            .Distinct()
            .CountAsync();

        // 在线人数（最近5分钟）
        var onlineCount = await _context.VisitorAnalytics
            .Where(v => v.IsOnline && v.UpdatedAt >= DateTime.Now.AddMinutes(-5))
            .Select(v => v.VisitorId)
            .Distinct()
            .CountAsync();

        // 热门文章（通过路径统计）
        var topArticles = await _context.VisitorAnalytics
            .Where(v => v.Path != null && v.Path.StartsWith("/blog/"))
            .GroupBy(v => v.Path)
            .Select(g => new { Path = g.Key, Views = g.Sum(v => v.PageViews) })
            .OrderByDescending(x => x.Views)
            .Take(10)
            .ToListAsync();

        // 访问区域统计
        var regionStats = await _context.VisitorAnalytics
            .Where(v => v.Country != null)
            .GroupBy(v => v.Country)
            .Select(g => new { Country = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        // 搜索来源
        var searchSources = await _context.VisitorAnalytics
            .Where(v => !string.IsNullOrEmpty(v.SearchKeyword))
            .GroupBy(v => v.SearchKeyword)
            .Select(g => new { Keyword = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        // 设备类型统计
        var deviceStats = await _context.VisitorAnalytics
            .Where(v => !string.IsNullOrEmpty(v.DeviceType))
            .GroupBy(v => v.DeviceType)
            .Select(g => new { DeviceType = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        // 浏览器统计
        var browserStats = await _context.VisitorAnalytics
            .Where(v => !string.IsNullOrEmpty(v.Browser))
            .GroupBy(v => v.Browser)
            .Select(g => new { Browser = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        // 操作系统统计
        var osStats = await _context.VisitorAnalytics
            .Where(v => !string.IsNullOrEmpty(v.Os))
            .GroupBy(v => v.Os)
            .Select(g => new { Os = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        return Ok(ApiResponse.Success(new
        {
            Today = new { Pv = todayPv, Uv = todayUv },
            Yesterday = new { Pv = yesterdayPv, Uv = yesterdayUv },
            OnlineCount = onlineCount,
            TopArticles = topArticles,
            RegionStats = regionStats,
            SearchSources = searchSources,
            DeviceStats = deviceStats,
            BrowserStats = browserStats,
            OsStats = osStats
        }));
    }

    private (string DeviceType, string Browser, string Os) ParseDeviceInfo(string userAgent)
    {
        var deviceType = "desktop";
        var browser = "unknown";
        var os = "unknown";

        if (userAgent.Contains("Mobile") || userAgent.Contains("Android"))
        {
            deviceType = "mobile";
        }
        else if (userAgent.Contains("Tablet") || userAgent.Contains("iPad"))
        {
            deviceType = "tablet";
        }

        if (userAgent.Contains("Chrome"))
        {
            browser = "Chrome";
        }
        else if (userAgent.Contains("Firefox"))
        {
            browser = "Firefox";
        }
        else if (userAgent.Contains("Safari"))
        {
            browser = "Safari";
        }
        else if (userAgent.Contains("Edge"))
        {
            browser = "Edge";
        }

        if (userAgent.Contains("Windows"))
        {
            os = "Windows";
        }
        else if (userAgent.Contains("Mac"))
        {
            os = "macOS";
        }
        else if (userAgent.Contains("Linux"))
        {
            os = "Linux";
        }
        else if (userAgent.Contains("Android"))
        {
            os = "Android";
        }
        else if (userAgent.Contains("iOS"))
        {
            os = "iOS";
        }

        return (deviceType, browser, os);
    }

    /// <summary>
    /// 获取IP地理位置信息（使用免费的ip-api.com服务）
    /// </summary>
    private async Task<(string? Country, string? Region, string? City)> GetIpGeoLocation(string? ip)
    {
        if (string.IsNullOrEmpty(ip) || ip == "::1" || ip == "127.0.0.1" || ip.StartsWith("192.168.") || ip.StartsWith("10.") || ip.StartsWith("172."))
        {
            return (null, null, null);
        }

        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(3);
            var response = await httpClient.GetStringAsync($"http://ip-api.com/json/{ip}?fields=status,country,regionName,city");
            var jsonDoc = JsonDocument.Parse(response);
            var root = jsonDoc.RootElement;

            if (root.GetProperty("status").GetString() == "success")
            {
                return (
                    root.TryGetProperty("country", out var country) ? country.GetString() : null,
                    root.TryGetProperty("regionName", out var region) ? region.GetString() : null,
                    root.TryGetProperty("city", out var city) ? city.GetString() : null
                );
            }
        }
        catch
        {
            // 静默失败，不影响主流程
        }

        return (null, null, null);
    }

    /// <summary>
    /// 获取访客列表（管理员）
    /// </summary>
    [HttpGet("visitors")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetVisitors(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool onlineOnly = false)
    {
        try
        {
            var query = _context.VisitorAnalytics.AsQueryable();

            // 只显示在线访客
            if (onlineOnly)
            {
                query = query.Where(v => v.IsOnline && v.UpdatedAt >= DateTime.Now.AddMinutes(-5));
            }

            // 按更新时间倒序排列
            query = query.OrderByDescending(v => v.UpdatedAt);

            var total = await query.CountAsync();
            var visitors = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new
                {
                    v.Id,
                    v.VisitorId,
                    v.Ip,
                    v.Country,
                    v.Region,
                    v.City,
                    v.DeviceType,
                    v.Browser,
                    v.Os,
                    v.Path,
                    v.Referrer,
                    v.SearchKeyword,
                    v.PageViews,
                    v.SessionStart,
                    v.UpdatedAt,
                    v.IsOnline
                })
                .ToListAsync();

            return Ok(ApiResponse.Success(new
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Visitors = visitors
            }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"获取访客列表失败: {ex.Message}", 500));
        }
    }
}

public class TrackRequest
{
    public string VisitorId { get; set; } = string.Empty;
    public string? Path { get; set; }
    public string? SearchKeyword { get; set; }
}

