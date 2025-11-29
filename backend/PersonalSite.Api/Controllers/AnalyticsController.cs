using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

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
    /// 同时写入 VisitLogs（基础日志）和 VisitorAnalytics（详细分析）
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

            // ========== 1. 写入 VisitLogs 表（基础访问日志）==========
            var visitLog = new VisitLog
            {
                Id = Guid.NewGuid().ToString(),
                VisitorId = request.VisitorId,
                Ip = ip,
                UserAgent = userAgent,
                Path = request.Path,
                Timestamp = DateTime.Now
            };
            _context.VisitLogs.Add(visitLog);

            // ========== 2. 写入或更新 VisitorAnalytics 表（详细分析）==========
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

            // 保存所有更改
            await _context.SaveChangesAsync();

            // 记录日志（用于调试）
            Console.WriteLine($"[Analytics] Track visit: VisitorId={request.VisitorId}, Path={request.Path}, IP={ip}, Time={DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            return Ok(ApiResponse.Success(new { SessionId = analytics.Id, VisitLogId = visitLog.Id }));
        }
        catch (Exception ex)
        {
            // 记录错误日志
            Console.WriteLine($"[Analytics] Track failed: {ex.Message}\n{ex.StackTrace}");
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

        // 从 VisitLogs 表获取基础统计数据（首页使用的表）
        var todayPvFromLogs = await _context.VisitLogs
            .Where(v => v.Timestamp >= today)
            .CountAsync();

        var todayUvFromLogs = await _context.VisitLogs
            .Where(v => v.Timestamp >= today)
            .Select(v => v.VisitorId)
            .Distinct()
            .CountAsync();

        var yesterdayPvFromLogs = await _context.VisitLogs
            .Where(v => v.Timestamp >= yesterday && v.Timestamp < today)
            .CountAsync();

        var yesterdayUvFromLogs = await _context.VisitLogs
            .Where(v => v.Timestamp >= yesterday && v.Timestamp < today)
            .Select(v => v.VisitorId)
            .Distinct()
            .CountAsync();

        // 从 VisitorAnalytics 表获取详细统计数据
        var todayPvFromAnalytics = await _context.VisitorAnalytics
            .Where(v => v.SessionStart >= today)
            .SumAsync(v => v.PageViews);

        var todayUvFromAnalytics = await _context.VisitorAnalytics
            .Where(v => v.SessionStart >= today)
            .Select(v => v.VisitorId)
            .Distinct()
            .CountAsync();

        var yesterdayPvFromAnalytics = await _context.VisitorAnalytics
            .Where(v => v.SessionStart >= yesterday && v.SessionStart < today)
            .SumAsync(v => v.PageViews);

        var yesterdayUvFromAnalytics = await _context.VisitorAnalytics
            .Where(v => v.SessionStart >= yesterday && v.SessionStart < today)
            .Select(v => v.VisitorId)
            .Distinct()
            .CountAsync();

        // 合并数据：优先使用 VisitorAnalytics，如果没有则使用 VisitLogs
        var todayPv = todayPvFromAnalytics > 0 ? todayPvFromAnalytics : todayPvFromLogs;
        var todayUv = todayUvFromAnalytics > 0 ? todayUvFromAnalytics : todayUvFromLogs;
        var yesterdayPv = yesterdayPvFromAnalytics > 0 ? yesterdayPvFromAnalytics : yesterdayPvFromLogs;
        var yesterdayUv = yesterdayUvFromAnalytics > 0 ? yesterdayUvFromAnalytics : yesterdayUvFromLogs;

        // 在线人数（最近5分钟）- 仅从 VisitorAnalytics
        var onlineCount = await _context.VisitorAnalytics
            .Where(v => v.IsOnline && v.UpdatedAt >= DateTime.Now.AddMinutes(-5))
            .Select(v => v.VisitorId)
            .Distinct()
            .CountAsync();

        // 热门文章（通过路径统计）- 优先从 VisitorAnalytics，否则从 VisitLogs
        var topArticlesFromAnalytics = await _context.VisitorAnalytics
            .Where(v => v.Path != null && v.Path.StartsWith("/blog/"))
            .GroupBy(v => v.Path)
            .Select(g => new { Path = g.Key, Views = g.Sum(v => v.PageViews) })
            .OrderByDescending(x => x.Views)
            .Take(10)
            .ToListAsync();

        var topArticlesFromLogs = await _context.VisitLogs
            .Where(v => v.Path != null && v.Path.StartsWith("/blog/"))
            .GroupBy(v => v.Path)
            .Select(g => new { Path = g.Key, Views = g.Count() })
            .OrderByDescending(x => x.Views)
            .Take(10)
            .ToListAsync();

        var topArticles = topArticlesFromAnalytics.Count > 0 ? topArticlesFromAnalytics : topArticlesFromLogs;

        // 访问区域统计 - 从 VisitorAnalytics，如果没有数据则尝试从 VisitLogs（需要解析IP）
        var regionStats = await _context.VisitorAnalytics
            .Where(v => v.Country != null)
            .GroupBy(v => v.Country)
            .Select(g => new { Country = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();
        
        // 如果 VisitorAnalytics 没有区域数据，返回空列表（因为 VisitLogs 没有地理位置信息）

        // 搜索来源
        var searchSources = await _context.VisitorAnalytics
            .Where(v => !string.IsNullOrEmpty(v.SearchKeyword))
            .GroupBy(v => v.SearchKeyword)
            .Select(g => new { Keyword = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        // 设备类型统计 - 优先从 VisitorAnalytics，否则从 VisitLogs 解析
        var deviceStatsFromAnalytics = await _context.VisitorAnalytics
            .Where(v => !string.IsNullOrEmpty(v.DeviceType))
            .GroupBy(v => v.DeviceType)
            .Select(g => new { DeviceType = g.Key ?? "", Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        var deviceStats = deviceStatsFromAnalytics.Count > 0
            ? deviceStatsFromAnalytics
            : (await _context.VisitLogs
                .Where(v => !string.IsNullOrEmpty(v.UserAgent))
                .ToListAsync())
                .Select(log =>
                {
                    var deviceInfo = ParseDeviceInfo(log.UserAgent ?? "");
                    return new { DeviceType = deviceInfo.DeviceType ?? "", UserAgent = log.UserAgent };
                })
                .Where(x => !string.IsNullOrEmpty(x.DeviceType))
                .GroupBy(x => x.DeviceType)
                .Select(g => new { DeviceType = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

        // 浏览器统计 - 优先从 VisitorAnalytics，否则从 VisitLogs 解析
        var browserStatsFromAnalytics = await _context.VisitorAnalytics
            .Where(v => !string.IsNullOrEmpty(v.Browser))
            .GroupBy(v => v.Browser)
            .Select(g => new { Browser = g.Key ?? "", Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        var browserStats = browserStatsFromAnalytics.Count > 0
            ? browserStatsFromAnalytics
            : (await _context.VisitLogs
                .Where(v => !string.IsNullOrEmpty(v.UserAgent))
                .ToListAsync())
                .Select(log =>
                {
                    var deviceInfo = ParseDeviceInfo(log.UserAgent ?? "");
                    return new { Browser = deviceInfo.Browser ?? "", UserAgent = log.UserAgent };
                })
                .Where(x => !string.IsNullOrEmpty(x.Browser))
                .GroupBy(x => x.Browser)
                .Select(g => new { Browser = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

        // 操作系统统计 - 优先从 VisitorAnalytics，否则从 VisitLogs 解析
        var osStatsFromAnalytics = await _context.VisitorAnalytics
            .Where(v => !string.IsNullOrEmpty(v.Os))
            .GroupBy(v => v.Os)
            .Select(g => new { Os = g.Key ?? "", Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        var osStats = osStatsFromAnalytics.Count > 0
            ? osStatsFromAnalytics
            : (await _context.VisitLogs
                .Where(v => !string.IsNullOrEmpty(v.UserAgent))
                .ToListAsync())
                .Select(log =>
                {
                    var deviceInfo = ParseDeviceInfo(log.UserAgent ?? "");
                    return new { Os = deviceInfo.Os ?? "", UserAgent = log.UserAgent };
                })
                .Where(x => !string.IsNullOrEmpty(x.Os))
                .GroupBy(x => x.Os)
                .Select(g => new { Os = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

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

    /// <summary>
    /// 统一时间范围解析工具
    /// </summary>
    private (DateTime Start, DateTime End) ResolveRange(string? range)
    {
        var now = DateTime.Now;
        range = (range ?? "7d").ToLowerInvariant();

        return range switch
        {
            "today" => (now.Date, now.Date.AddDays(1)),
            "30d" => (now.Date.AddDays(-29), now.Date.AddDays(1)),
            "90d" => (now.Date.AddDays(-89), now.Date.AddDays(1)),
            _ => (now.Date.AddDays(-6), now.Date.AddDays(1)) // 默认 7d
        };
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
    /// 获取访问趋势统计（管理员）
    /// </summary>
    [HttpGet("trend")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetTrend(
        [FromQuery] string? range = "7d",
        [FromQuery] string? granularity = "day")
    {
        try
        {
            // 规范化参数
            var normalizedRange = (range ?? "7d").ToLowerInvariant();
            var normalizedGranularity = (granularity ?? "day").ToLowerInvariant();

            // 解析时间范围（使用统一方法）
            var (startDate, endDate) = ResolveRange(normalizedRange);
            
            // 确保时间范围有效
            if (startDate >= endDate)
            {
                Console.WriteLine($"[Analytics Trend] Invalid date range: {startDate} >= {endDate}");
                return Ok(ApiResponse.Success(new
                {
                    range = normalizedRange,
                    granularity = normalizedGranularity,
                    points = Array.Empty<object>()
                }));
            }

            List<TrendPoint> points = new List<TrendPoint>();

            // 优先从 VisitorAnalytics 获取数据
            var analyticsCount = await _context.VisitorAnalytics.CountAsync();
            var useAnalytics = analyticsCount > 0;

            if (useAnalytics && normalizedGranularity == "day")
            {
                // 从 VisitorAnalytics 按天统计
                try
                {
                    var analyticsQuery = _context.VisitorAnalytics
                        .Where(v => v.SessionStart >= startDate && v.SessionStart < endDate);

                    // 先检查是否有数据
                    var hasData = await analyticsQuery.AnyAsync();
                    if (!hasData)
                    {
                        Console.WriteLine("[Analytics Trend] No VisitorAnalytics data found");
                    }
                    else
                    {
                        // 先拉到内存，避免 EF Core 翻译问题
                        var analyticsData = await analyticsQuery
                            .Select(v => new { v.SessionStart, v.PageViews, v.VisitorId })
                            .ToListAsync();

                        points = analyticsData
                            .GroupBy(v => v.SessionStart.Date)
                            .Select(g => new TrendPoint
                            {
                                Date = g.Key.ToString("yyyy-MM-dd"),
                                Pv = g.Sum(v => v.PageViews),
                                Uv = g.Select(v => v.VisitorId).Distinct().Count()
                            })
                            .OrderBy(p => p.Date)
                            .ToList();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Analytics Trend] Error querying VisitorAnalytics: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    // 继续尝试从 VisitLogs 获取
                }
            }
            else if (useAnalytics && normalizedGranularity == "hour")
            {
                // 从 VisitorAnalytics 按小时统计（最近24小时）
                try
                {
                    var hourStart = DateTime.Now.AddHours(-24);
                    var analyticsQuery = _context.VisitorAnalytics
                        .Where(v => v.SessionStart >= hourStart && v.SessionStart < endDate);

                    var hasData = await analyticsQuery.AnyAsync();
                    if (!hasData)
                    {
                        Console.WriteLine("[Analytics Trend] No VisitorAnalytics hourly data found");
                    }
                    else
                    {
                        var analyticsData = await analyticsQuery
                            .Select(v => new { v.SessionStart, v.PageViews, v.VisitorId })
                            .ToListAsync();

                        points = analyticsData
                            .GroupBy(v => new { 
                                Date = v.SessionStart.Date, 
                                Hour = v.SessionStart.Hour 
                            })
                            .Select(g => new TrendPoint
                            {
                                Date = g.Key.Date.ToString("yyyy-MM-dd") + " " + g.Key.Hour.ToString("00") + ":00",
                                Pv = g.Sum(v => v.PageViews),
                                Uv = g.Select(v => v.VisitorId).Distinct().Count()
                            })
                            .OrderBy(p => p.Date)
                            .ToList();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Analytics Trend] Error querying VisitorAnalytics hourly: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
            }

            // 如果 VisitorAnalytics 没有数据，从 VisitLogs 统计
            if (points.Count == 0)
            {
                try
                {
                    var logsQuery = _context.VisitLogs
                        .Where(v => v.Timestamp >= startDate && v.Timestamp < endDate);

                    var hasData = await logsQuery.AnyAsync();
                    if (!hasData)
                    {
                        Console.WriteLine("[Analytics Trend] No VisitLogs data found");
                    }
                    else
                    {
                        if (normalizedGranularity == "day")
                        {
                            // 先拉到内存，避免 EF Core 翻译问题
                            var logsData = await logsQuery
                                .Select(v => new { v.Timestamp, v.VisitorId })
                                .ToListAsync();

                            points = logsData
                                .GroupBy(v => v.Timestamp.Date)
                                .Select(g => new TrendPoint
                                {
                                    Date = g.Key.ToString("yyyy-MM-dd"),
                                    Pv = g.Count(),
                                    Uv = g.Select(v => v.VisitorId).Distinct().Count()
                                })
                                .OrderBy(p => p.Date)
                                .ToList();
                        }
                        else // hour
                        {
                            var hourStart = DateTime.Now.AddHours(-24);
                            var hourLogsQuery = _context.VisitLogs
                                .Where(v => v.Timestamp >= hourStart && v.Timestamp < endDate);

                            var hourLogsData = await hourLogsQuery
                                .Select(v => new { v.Timestamp, v.VisitorId })
                                .ToListAsync();

                            points = hourLogsData
                                .GroupBy(v => new { 
                                    Date = v.Timestamp.Date, 
                                    Hour = v.Timestamp.Hour 
                                })
                                .Select(g => new TrendPoint
                                {
                                    Date = g.Key.Date.ToString("yyyy-MM-dd") + " " + g.Key.Hour.ToString("00") + ":00",
                                    Pv = g.Count(),
                                    Uv = g.Select(v => v.VisitorId).Distinct().Count()
                                })
                                .OrderBy(p => p.Date)
                                .ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Analytics Trend] Error querying VisitLogs: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    // 即使出错也返回空数组，不抛异常
                }
            }

            // 填充缺失的日期（确保连续）- 仅对按天统计且时间范围合理的情况
            if (normalizedGranularity == "day" && startDate.Date <= endDate.Date)
            {
                try
                {
                    var filledPoints = new List<TrendPoint>();
                    var currentDate = startDate.Date;
                    var endDateOnly = endDate.Date;
                    var pointDict = points.ToDictionary(p => p.Date);

                    // 限制填充范围，避免无限循环
                    var maxDays = 365; // 最多填充一年
                    var dayCount = 0;

                    while (currentDate <= endDateOnly && dayCount < maxDays)
                    {
                        var dateStr = currentDate.ToString("yyyy-MM-dd");
                        if (pointDict.ContainsKey(dateStr))
                        {
                            filledPoints.Add(pointDict[dateStr]);
                        }
                        else
                        {
                            filledPoints.Add(new TrendPoint
                            {
                                Date = dateStr,
                                Pv = 0,
                                Uv = 0
                            });
                        }
                        currentDate = currentDate.AddDays(1);
                        dayCount++;
                    }

                    points = filledPoints;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Analytics Trend] Error filling dates: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    // 即使填充失败，也返回已有数据
                }
            }

            // 确保返回的数据结构正确
            return Ok(ApiResponse.Success(new
            {
                range = normalizedRange,
                granularity = normalizedGranularity,
                points = points.Select(p => new
                {
                    date = p.Date,
                    pv = p.Pv,
                    uv = p.Uv
                }).ToArray()
            }));
        }
        catch (Exception ex)
        {
            // 详细错误日志
            Console.WriteLine("=== Analytics Trend Error ===");
            Console.WriteLine($"Range: {range}, Granularity: {granularity}");
            Console.WriteLine($"Exception: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            Console.WriteLine("============================");

            // 返回 200，但包含错误信息（方便调试）
            // 注意：ApiResponse.Error 不支持 data 参数，所以直接返回 Success 但包含错误信息
            return Ok(ApiResponse<object>.Success(new
            {
                range = range ?? "7d",
                granularity = granularity ?? "day",
                points = Array.Empty<object>(),
                error = ex.Message,
                stack = ex.StackTrace
            }, "Trend calculation failed"));
        }
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
            // 优先从 VisitorAnalytics 获取
            var analyticsCount = await _context.VisitorAnalytics.CountAsync();
            
            if (analyticsCount > 0)
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
            else
            {
                // 如果 VisitorAnalytics 没有数据，从 VisitLogs 读取
                var query = _context.VisitLogs.AsQueryable();

                // 只显示最近24小时的访客（因为 VisitLogs 没有在线状态）
                if (onlineOnly)
                {
                    query = query.Where(v => v.Timestamp >= DateTime.Now.AddHours(-24));
                }

                // 按时间倒序排列
                query = query.OrderByDescending(v => v.Timestamp);

                var total = await query.CountAsync();
                var visitLogs = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // 转换为访客格式
                var visitors = visitLogs.Select(v =>
                {
                    var deviceInfo = ParseDeviceInfo(v.UserAgent ?? "");
                    return new
                    {
                        Id = v.Id,
                        VisitorId = v.VisitorId,
                        Ip = v.Ip,
                        Country = (string?)null,
                        Region = (string?)null,
                        City = (string?)null,
                        DeviceType = deviceInfo.DeviceType,
                        Browser = deviceInfo.Browser,
                        Os = deviceInfo.Os,
                        Path = v.Path,
                        Referrer = (string?)null,
                        SearchKeyword = (string?)null,
                        PageViews = 1,
                        SessionStart = v.Timestamp,
                        UpdatedAt = v.Timestamp,
                        IsOnline = !onlineOnly || v.Timestamp >= DateTime.Now.AddMinutes(-5)
                    };
                }).ToList();

                return Ok(ApiResponse.Success(new
                {
                    Total = total,
                    Page = page,
                    PageSize = pageSize,
                    Visitors = visitors
                }));
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"获取访客列表失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 调试接口：获取数据链路状态（仅管理员）
    /// </summary>
    [HttpGet("debug-status")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetDebugStatus()
    {
        try
        {
            var today = DateTime.Today;
            var last7Days = DateTime.Today.AddDays(-7);

            // VisitLogs 统计
            var visitLogsCountLast7Days = await _context.VisitLogs
                .Where(v => v.Timestamp >= last7Days)
                .CountAsync();

            var visitLogsCountToday = await _context.VisitLogs
                .Where(v => v.Timestamp >= today)
                .CountAsync();

            // VisitorAnalytics 统计
            var visitorAnalyticsCount = await _context.VisitorAnalytics.CountAsync();
            var hasVisitorAnalyticsData = visitorAnalyticsCount > 0;

            // 今日 PV/UV
            var todayPvFromLogs = await _context.VisitLogs
                .Where(v => v.Timestamp >= today)
                .CountAsync();

            var todayUvFromLogs = await _context.VisitLogs
                .Where(v => v.Timestamp >= today)
                .Select(v => v.VisitorId)
                .Distinct()
                .CountAsync();

            var todayPvFromAnalytics = await _context.VisitorAnalytics
                .Where(v => v.SessionStart >= today)
                .SumAsync(v => v.PageViews);

            var todayUvFromAnalytics = await _context.VisitorAnalytics
                .Where(v => v.SessionStart >= today)
                .Select(v => v.VisitorId)
                .Distinct()
                .CountAsync();

            // 最后一条访问记录
            var lastVisitLog = await _context.VisitLogs
                .OrderByDescending(v => v.Timestamp)
                .FirstOrDefaultAsync();

            var lastVisitSample = lastVisitLog != null
                ? new
                {
                    url = lastVisitLog.Path,
                    ip = lastVisitLog.Ip,
                    time = lastVisitLog.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    visitorId = lastVisitLog.VisitorId
                }
                : null;

            return Ok(ApiResponse.Success(new
            {
                visitLogsCountLast7Days,
                visitLogsCountToday,
                hasVisitorAnalyticsData,
                visitorAnalyticsCount,
                todayPv = todayPvFromAnalytics > 0 ? todayPvFromAnalytics : todayPvFromLogs,
                todayUv = todayUvFromAnalytics > 0 ? todayUvFromAnalytics : todayUvFromLogs,
                todayPvFromLogs,
                todayUvFromLogs,
                todayPvFromAnalytics,
                todayUvFromAnalytics,
                lastVisitSample
            }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"获取调试状态失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 概览接口：获取今日/昨日/总计 PV/UV 等核心指标
    /// </summary>
    [HttpGet("overview")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetOverview()
    {
        try
        {
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);

            // 今日 PV/UV（从 VisitLogs）
            var todayPv = await _context.VisitLogs
                .Where(v => v.Timestamp >= today)
                .CountAsync();

            var todayUv = await _context.VisitLogs
                .Where(v => v.Timestamp >= today)
                .Select(v => v.VisitorId)
                .Distinct()
                .CountAsync();

            // 昨日 PV/UV
            var yesterdayPv = await _context.VisitLogs
                .Where(v => v.Timestamp >= yesterday && v.Timestamp < today)
                .CountAsync();

            var yesterdayUv = await _context.VisitLogs
                .Where(v => v.Timestamp >= yesterday && v.Timestamp < today)
                .Select(v => v.VisitorId)
                .Distinct()
                .CountAsync();

            // 总计 PV/UV
            var totalPv = await _context.VisitLogs.CountAsync();
            var totalUv = await _context.VisitLogs
                .Select(v => v.VisitorId)
                .Distinct()
                .CountAsync();

            // 在线人数（最近 5 分钟）
            var onlineUsers = await _context.VisitLogs
                .Where(v => v.Timestamp >= DateTime.Now.AddMinutes(-5))
                .Select(v => v.VisitorId)
                .Distinct()
                .CountAsync();

            // 热门文章数（访问次数大于 1 的不同 URL 数量，以 /article/ 或 /blog/ 开头）
            var hotArticleCount = await _context.VisitLogs
                .Where(v => v.Path != null && (v.Path.StartsWith("/article/") || v.Path.StartsWith("/blog/")))
                .GroupBy(v => v.Path)
                .Where(g => g.Count() > 1)
                .CountAsync();

            return Ok(ApiResponse.Success(new
            {
                todayPv,
                todayUv,
                yesterdayPv,
                yesterdayUv,
                totalPv,
                totalUv,
                onlineUsers,
                hotArticleCount
            }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"获取概览数据失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// Top 页面接口：获取访问量最高的页面列表
    /// </summary>
    [HttpGet("top-pages")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetTopPages([FromQuery] string? range = "7d")
    {
        try
        {
            var (startDate, endDate) = ResolveRange(range);

            var topPages = await _context.VisitLogs
                .Where(v => v.Timestamp >= startDate && v.Timestamp < endDate && v.Path != null)
                .GroupBy(v => v.Path)
                .Select(g => new
                {
                    url = g.Key,
                    title = (string?)null, // 后续可从文章表关联获取
                    pv = g.Count(),
                    uv = g.Select(v => v.VisitorId).Distinct().Count(),
                    avgDuration = 0 // 暂时填 0，后续可做停留时长
                })
                .OrderByDescending(x => x.pv)
                .Take(50)
                .ToListAsync();

            return Ok(ApiResponse.Success(new { items = topPages }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"获取 Top 页面失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 来源分析接口：分析访问来源（直接访问/搜索引擎/外部链接/站内跳转）
    /// </summary>
    [HttpGet("sources")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetSources([FromQuery] string? range = "7d")
    {
        try
        {
            var (startDate, endDate) = ResolveRange(range);

            // 获取时间范围内的访问记录（需要 Referrer 信息）
            // 注意：VisitLogs 表可能没有 Referrer 字段，需要从 VisitorAnalytics 获取
            var analyticsWithReferrer = await _context.VisitorAnalytics
                .Where(v => v.SessionStart >= startDate && v.SessionStart < endDate && !string.IsNullOrEmpty(v.Referrer))
                .Select(v => new { v.Referrer, v.VisitorId })
                .ToListAsync();

            // 如果 VisitorAnalytics 没有数据，尝试从 VisitLogs 获取（但 VisitLogs 可能没有 Referrer）
            // 这里先假设从 VisitorAnalytics 获取，如果没有数据则返回空统计

            var directCount = 0;
            var searchCount = 0;
            var externalCount = 0;
            var internalCount = 0;
            var referrerDict = new Dictionary<string, int>();

            // 获取当前站点域名（从配置或请求头获取）
            var currentHost = Request.Headers["Host"].ToString();
            if (string.IsNullOrEmpty(currentHost))
            {
                currentHost = "localhost"; // 默认值
            }

            // 搜索引擎域名列表
            var searchEngines = new[] { "baidu.com", "google.com", "bing.com", "so.com", "sogou.com", "yandex.com" };

            foreach (var item in analyticsWithReferrer)
            {
                if (string.IsNullOrEmpty(item.Referrer) || item.Referrer == "-")
                {
                    directCount++;
                    continue;
                }

                try
                {
                    if (Uri.TryCreate(item.Referrer, UriKind.Absolute, out var uri))
                    {
                        var host = uri.Host.ToLowerInvariant();

                        // 站内跳转
                        if (host.Contains(currentHost) || host == "localhost" || host == "127.0.0.1")
                        {
                            internalCount++;
                        }
                        // 搜索引擎
                        else if (searchEngines.Any(se => host.Contains(se)))
                        {
                            searchCount++;
                            referrerDict[host] = referrerDict.GetValueOrDefault(host, 0) + 1;
                        }
                        // 外部链接
                        else
                        {
                            externalCount++;
                            referrerDict[host] = referrerDict.GetValueOrDefault(host, 0) + 1;
                        }
                    }
                    else
                    {
                        directCount++;
                    }
                }
                catch
                {
                    directCount++;
                }
            }

            // 如果没有从 VisitorAnalytics 获取到数据，尝试从 VisitLogs 统计（但 VisitLogs 可能没有 Referrer）
            if (analyticsWithReferrer.Count == 0)
            {
                // 假设所有访问都是直接访问（因为 VisitLogs 没有 Referrer 字段）
                var totalCount = await _context.VisitLogs
                    .Where(v => v.Timestamp >= startDate && v.Timestamp < endDate)
                    .CountAsync();
                directCount = totalCount;
            }

            var total = directCount + searchCount + externalCount + internalCount;

            var items = new List<object>
            {
                new { type = "direct", name = "直接访问", count = directCount },
                new { type = "search", name = "搜索引擎", count = searchCount },
                new { type = "external", name = "外部链接", count = externalCount },
                new { type = "internal", name = "站内跳转", count = internalCount }
            };

            var topReferrers = referrerDict
                .OrderByDescending(kvp => kvp.Value)
                .Take(10)
                .Select(kvp => new { host = kvp.Key, count = kvp.Value })
                .ToList();

            return Ok(ApiResponse.Success(new
            {
                total,
                items,
                topReferrers
            }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"获取来源分析失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 搜索关键词接口：从搜索引擎来源中提取搜索关键词
    /// </summary>
    [HttpGet("search-keywords")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetSearchKeywords([FromQuery] string? range = "7d")
    {
        try
        {
            var (startDate, endDate) = ResolveRange(range);

            // 从 VisitorAnalytics 获取有搜索关键词的记录
            var keywordsFromAnalytics = await _context.VisitorAnalytics
                .Where(v => v.SessionStart >= startDate && v.SessionStart < endDate && !string.IsNullOrEmpty(v.SearchKeyword))
                .GroupBy(v => v.SearchKeyword)
                .Select(g => new { keyword = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count)
                .Take(20)
                .ToListAsync();

            // 如果 VisitorAnalytics 有数据，直接返回
            if (keywordsFromAnalytics.Count > 0)
            {
                return Ok(ApiResponse.Success(new { items = keywordsFromAnalytics }));
            }

            // 否则尝试从来源为搜索引擎的访问中解析关键词
            // 这里需要从 Referrer URL 中提取查询参数
            var searchReferrers = await _context.VisitorAnalytics
                .Where(v => v.SessionStart >= startDate && v.SessionStart < endDate && !string.IsNullOrEmpty(v.Referrer))
                .Select(v => v.Referrer)
                .ToListAsync();

            var keywordDict = new Dictionary<string, int>();
            var searchEngines = new[] { "baidu.com", "google.com", "bing.com", "so.com", "sogou.com" };

            foreach (var referrer in searchReferrers)
            {
                try
                {
                    if (Uri.TryCreate(referrer, UriKind.Absolute, out var uri))
                    {
                        var host = uri.Host.ToLowerInvariant();
                        if (!searchEngines.Any(se => host.Contains(se))) continue;

                        var query = uri.Query;
                        var queryParams = QueryHelpers.ParseQuery(query);

                        string? keyword = null;

                        // 百度：wd 或 word
                        if (host.Contains("baidu.com"))
                        {
                            keyword = queryParams.ContainsKey("wd") ? queryParams["wd"].ToString() : 
                                     (queryParams.ContainsKey("word") ? queryParams["word"].ToString() : null);
                        }
                        // Google/Bing：q
                        else if (host.Contains("google.com") || host.Contains("bing.com"))
                        {
                            keyword = queryParams.ContainsKey("q") ? queryParams["q"].ToString() : null;
                        }
                        // 搜狗：query
                        else if (host.Contains("sogou.com"))
                        {
                            keyword = queryParams.ContainsKey("query") ? queryParams["query"].ToString() : null;
                        }

                        if (!string.IsNullOrEmpty(keyword))
                        {
                            keyword = Uri.UnescapeDataString(keyword);
                            if (!string.IsNullOrWhiteSpace(keyword))
                            {
                                keywordDict[keyword] = keywordDict.GetValueOrDefault(keyword, 0) + 1;
                            }
                        }
                    }
                }
                catch
                {
                    // 忽略解析错误
                }
            }

            var items = keywordDict
                .OrderByDescending(kvp => kvp.Value)
                .Take(20)
                .Select(kvp => new { keyword = kvp.Key, count = kvp.Value })
                .ToList();

            return Ok(ApiResponse.Success(new { items }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"获取搜索关键词失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 地区分布接口：按国家/省份统计访问分布
    /// </summary>
    [HttpGet("regions")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetRegions([FromQuery] string? range = "7d")
    {
        try
        {
            var (startDate, endDate) = ResolveRange(range);

            // 从 VisitorAnalytics 获取地区数据
            var regionsFromAnalytics = await _context.VisitorAnalytics
                .Where(v => v.SessionStart >= startDate && v.SessionStart < endDate && !string.IsNullOrEmpty(v.Country))
                .GroupBy(v => new { v.Country, v.Region })
                .Select(g => new
                {
                    country = g.Key.Country ?? "Unknown",
                    province = g.Key.Region ?? "",
                    count = g.Count()
                })
                .OrderByDescending(x => x.count)
                .ToListAsync();

            if (regionsFromAnalytics.Count > 0)
            {
                return Ok(ApiResponse.Success(new { items = regionsFromAnalytics }));
            }

            // 如果 VisitorAnalytics 没有地区数据，返回空列表
            // 后续可以通过 IP 解析库补充
            return Ok(ApiResponse.Success(new { items = new List<object>() }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"获取地区分布失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 客户端分布接口：设备类型/浏览器/操作系统统计
    /// </summary>
    [HttpGet("client-distribution")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetClientDistribution([FromQuery] string? range = "7d")
    {
        try
        {
            var (startDate, endDate) = ResolveRange(range);

            // 从 VisitorAnalytics 获取（如果存在）
            var analyticsData = await _context.VisitorAnalytics
                .Where(v => v.SessionStart >= startDate && v.SessionStart < endDate)
                .ToListAsync();

            List<object> devices, browsers, os;

            if (analyticsData.Count > 0)
            {
                // 从 VisitorAnalytics 统计
                devices = analyticsData
                    .Where(v => !string.IsNullOrEmpty(v.DeviceType))
                    .GroupBy(v => v.DeviceType)
                    .Select(g => new { type = (g.Key ?? "").ToLowerInvariant(), name = GetDeviceName(g.Key ?? ""), count = g.Count() })
                    .OrderByDescending(x => x.count)
                    .Cast<object>()
                    .ToList();

                browsers = analyticsData
                    .Where(v => !string.IsNullOrEmpty(v.Browser))
                    .GroupBy(v => NormalizeBrowser(v.Browser ?? ""))
                    .Select(g => new { type = g.Key, name = GetBrowserName(g.Key), count = g.Count() })
                    .OrderByDescending(x => x.count)
                    .Cast<object>()
                    .ToList();

                os = analyticsData
                    .Where(v => !string.IsNullOrEmpty(v.Os))
                    .GroupBy(v => NormalizeOs(v.Os ?? ""))
                    .Select(g => new { type = g.Key, name = GetOsName(g.Key), count = g.Count() })
                    .OrderByDescending(x => x.count)
                    .Cast<object>()
                    .ToList();
            }
            else
            {
                // 从 VisitLogs 解析
                var logs = await _context.VisitLogs
                    .Where(v => v.Timestamp >= startDate && v.Timestamp < endDate && !string.IsNullOrEmpty(v.UserAgent))
                    .ToListAsync();

                var deviceDict = new Dictionary<string, int>();
                var browserDict = new Dictionary<string, int>();
                var osDict = new Dictionary<string, int>();

                foreach (var log in logs)
                {
                    var deviceInfo = ParseDeviceInfo(log.UserAgent ?? "");

                    // 设备类型
                    var deviceType = (deviceInfo.DeviceType ?? "").ToLowerInvariant();
                    deviceDict[deviceType] = deviceDict.GetValueOrDefault(deviceType, 0) + 1;

                    // 浏览器
                    var browser = NormalizeBrowser(deviceInfo.Browser ?? "");
                    browserDict[browser] = browserDict.GetValueOrDefault(browser, 0) + 1;

                    // 操作系统
                    var osType = NormalizeOs(deviceInfo.Os ?? "");
                    osDict[osType] = osDict.GetValueOrDefault(osType, 0) + 1;
                }

                devices = deviceDict
                    .Select(kvp => new { type = kvp.Key, name = GetDeviceName(kvp.Key), count = kvp.Value })
                    .OrderByDescending(x => x.count)
                    .Cast<object>()
                    .ToList();

                browsers = browserDict
                    .Select(kvp => new { type = kvp.Key, name = GetBrowserName(kvp.Key), count = kvp.Value })
                    .OrderByDescending(x => x.count)
                    .Cast<object>()
                    .ToList();

                os = osDict
                    .Select(kvp => new { type = kvp.Key, name = GetOsName(kvp.Key), count = kvp.Value })
                    .OrderByDescending(x => x.count)
                    .Cast<object>()
                    .ToList();
            }

            return Ok(ApiResponse.Success(new { devices, browsers, os }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"获取客户端分布失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 行为路径接口：分析页面访问流程
    /// </summary>
    [HttpGet("page-flow")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetPageFlow([FromQuery] string? range = "7d", [FromQuery] int maxNodes = 20)
    {
        try
        {
            var (startDate, endDate) = ResolveRange(range);

            // 获取时间范围内的访问记录，按 VisitorId 和时间排序
            var visits = await _context.VisitLogs
                .Where(v => v.Timestamp >= startDate && v.Timestamp < endDate && v.Path != null)
                .OrderBy(v => v.VisitorId)
                .ThenBy(v => v.Timestamp)
                .Select(v => new { v.VisitorId, v.Path, v.Timestamp })
                .ToListAsync();

            // 按会话分组（30 分钟内视为同一会话）
            var sessions = new List<List<(string Path, DateTime Time)>>();
            var currentSession = new List<(string Path, DateTime Time)>();
            string? lastVisitorId = null;
            DateTime? lastTime = null;

            foreach (var visit in visits)
            {
                if (lastVisitorId != visit.VisitorId || (lastTime.HasValue && (visit.Timestamp - lastTime.Value).TotalMinutes > 30))
                {
                    if (currentSession.Count > 0)
                    {
                        sessions.Add(currentSession);
                    }
                    currentSession = new List<(string Path, DateTime Time)>();
                }

                currentSession.Add((visit.Path ?? "/", visit.Timestamp));
                lastVisitorId = visit.VisitorId;
                lastTime = visit.Timestamp;
            }

            if (currentSession.Count > 0)
            {
                sessions.Add(currentSession);
            }

            // 统计节点和边
            var nodeDict = new Dictionary<string, int>();
            var edgeDict = new Dictionary<(string From, string To), int>();

            foreach (var session in sessions)
            {
                for (int i = 0; i < session.Count; i++)
                {
                    var path = session[i].Path;
                    var nodeId = path == "/" ? "landing:/" : $"page:{path}";

                    // 统计节点（作为起点）
                    if (i == 0)
                    {
                        nodeDict[nodeId] = nodeDict.GetValueOrDefault(nodeId, 0) + 1;
                    }

                    // 统计边（页面跳转）
                    if (i < session.Count - 1)
                    {
                        var nextPath = session[i + 1].Path;
                        var nextNodeId = nextPath == "/" ? "landing:/" : $"page:{nextPath}";
                        var edgeKey = (nodeId, nextNodeId);
                        edgeDict[edgeKey] = edgeDict.GetValueOrDefault(edgeKey, 0) + 1;
                    }
                }
            }

            // 取 Top N 节点
            var topNodes = nodeDict
                .OrderByDescending(kvp => kvp.Value)
                .Take(maxNodes)
                .Select(kvp => new { id = kvp.Key, label = kvp.Key.Replace("landing:", "").Replace("page:", ""), count = kvp.Value })
                .ToList();

            var topNodeIds = topNodes.Select(n => n.id).ToHashSet();

            // 只保留与 Top 节点相关的边
            var topEdges = edgeDict
                .Where(kvp => topNodeIds.Contains(kvp.Key.From) && topNodeIds.Contains(kvp.Key.To))
                .OrderByDescending(kvp => kvp.Value)
                .Take(50)
                .Select(kvp => new { from = kvp.Key.From, to = kvp.Key.To, count = kvp.Value })
                .ToList();

            return Ok(ApiResponse.Success(new { nodes = topNodes, edges = topEdges }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"获取行为路径失败: {ex.Message}", 500));
        }
    }

    // 辅助方法：规范化浏览器名称
    private string NormalizeBrowser(string? browser)
    {
        if (string.IsNullOrEmpty(browser))
        {
            return "other";
        }
        var b = browser.ToLowerInvariant();
        if (b.Contains("chrome") && !b.Contains("edg")) return "chrome";
        if (b.Contains("safari") && !b.Contains("chrome")) return "safari";
        if (b.Contains("firefox")) return "firefox";
        if (b.Contains("edg")) return "edge";
        if (b.Contains("micromessenger")) return "wechat";
        return "other";
    }

    // 辅助方法：规范化操作系统名称
    private string NormalizeOs(string? os)
    {
        if (string.IsNullOrEmpty(os))
        {
            return "other";
        }
        var o = os.ToLowerInvariant();
        if (o.Contains("windows")) return "windows";
        if (o.Contains("mac")) return "macos";
        if (o.Contains("android")) return "android";
        if (o.Contains("ios")) return "ios";
        if (o.Contains("linux")) return "linux";
        return "other";
    }

    // 辅助方法：获取设备类型中文名称
    private string GetDeviceName(string deviceType)
    {
        return deviceType.ToLowerInvariant() switch
        {
            "desktop" => "电脑",
            "mobile" => "手机",
            "tablet" => "平板",
            _ => "未知"
        };
    }

    // 辅助方法：获取浏览器中文名称
    private string GetBrowserName(string browser)
    {
        return browser.ToLowerInvariant() switch
        {
            "chrome" => "Chrome",
            "safari" => "Safari",
            "firefox" => "Firefox",
            "edge" => "Edge",
            "wechat" => "微信内置浏览器",
            _ => "其他"
        };
    }

    // 辅助方法：获取操作系统中文名称
    private string GetOsName(string os)
    {
        return os.ToLowerInvariant() switch
        {
            "windows" => "Windows",
            "macos" => "macOS",
            "android" => "Android",
            "ios" => "iOS",
            "linux" => "Linux",
            _ => "其他"
        };
    }
}

public class TrackRequest
{
    public string VisitorId { get; set; } = string.Empty;
    public string? Path { get; set; }
    public string? SearchKeyword { get; set; }
}

public class TrendPoint
{
    public string Date { get; set; } = string.Empty;
    public int Pv { get; set; }
    public int Uv { get; set; }
}

