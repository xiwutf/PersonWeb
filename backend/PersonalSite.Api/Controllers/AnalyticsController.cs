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
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(AppDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<AnalyticsController> logger)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
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

            // 修复访客列表 IP 一直显示未知的问题：使用统一的 IP 获取方法，优先从 X-Forwarded-For 获取
            // 注意：即使是私网 IP 也会返回，只是后续不解析地理位置
            var ip = GetClientIpAddress();
            
            var userAgent = Request.Headers["User-Agent"].ToString();
            var referrer = Request.Headers["Referer"].ToString();

            // 解析设备信息
            var deviceInfo = ParseDeviceInfo(userAgent);

            // ========== 1. 写入 VisitLogs 表（基础访问日志）==========
            // 修复访客列表 IP 一直显示未知的问题：确保 IP 字段正确写入，即使是私网 IP 也要记录
            var visitLog = new VisitLog
            {
                Id = Guid.NewGuid().ToString(),
                VisitorId = request.VisitorId,
                Ip = ip, // IP 可能为 null（如果完全获取不到），或私网 IP（会记录但地理位置显示为未知）
                UserAgent = userAgent,
                Path = request.Path ?? "/", // 确保 Path 不为 null
                Timestamp = DateTime.Now
            };
            _context.VisitLogs.Add(visitLog);

            // ========== 2. 写入或更新 VisitorAnalytics 表（详细分析）==========
            // 查找或创建访客会话
            // 查找最近30分钟内有活动的会话（即使 IsOnline=false，也可能还在线）
            var thirtyMinutesAgo = DateTime.Now.AddMinutes(-30);
            var analytics = await _context.VisitorAnalytics
                .Where(v => v.VisitorId == request.VisitorId && v.UpdatedAt >= thirtyMinutesAgo)
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

            return Ok(ApiResponse.Success(new { SessionId = analytics.Id, VisitLogId = visitLog.Id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Analytics track failed");
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
    /// 获取客户端真实 IP 地址（修复访客列表 IP 一直显示未知的问题）
    /// 优先级：X-Forwarded-For（第一个非私网IP） > X-Real-IP > RemoteIpAddress
    /// 注意：即使是私网 IP 也会返回，只是后续不解析地理位置（前端会显示为"未知"）
    /// </summary>
    private string? GetClientIpAddress()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        // 1. 优先从 X-Forwarded-For 获取（可能包含多个IP，用逗号分隔）
        // X-Forwarded-For 格式：client, proxy1, proxy2
        var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            // 遍历所有 IP，返回第一个非私网 IP
            foreach (var ip in ips)
            {
                var trimmedIp = ip.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedIp) && !IsPrivateIp(trimmedIp))
                {
                    return trimmedIp;
                }
            }
            
            // 如果都是私网 IP，返回第一个（仍然记录，但地理位置显示为未知）
            // 修复访客列表 IP 一直显示未知的问题：确保私网 IP 也会返回，而不是 null
            if (ips.Length > 0)
            {
                var firstIp = ips[0].Trim();
                if (!string.IsNullOrWhiteSpace(firstIp))
                {
                    return firstIp;
                }
            }
        }

        // 2. 其次从 X-Real-IP 获取
        var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            var trimmedIp = realIp.Trim();
            // 修复访客列表 IP 一直显示未知的问题：即使是私网 IP 也返回，但地理位置会显示为未知
            if (!string.IsNullOrWhiteSpace(trimmedIp))
            {
                return trimmedIp;
            }
        }

        // 3. 最后从 RemoteIpAddress 获取
        var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();
        // 修复访客列表 IP 一直显示未知的问题：即使获取不到 IP 也返回 null（前端会显示为"未知"）
        return remoteIp;
    }

    /// <summary>
    /// 判断是否为私网 IP（修复线上访问仍然显示未知的问题）
    /// </summary>
    private bool IsPrivateIp(string? ip)
    {
        if (string.IsNullOrEmpty(ip)) return true;

        // IPv4 私网地址范围检查
        // 192.168.0.0/16
        if (ip.StartsWith("192.168."))
        {
            return true;
        }
        
        // 10.0.0.0/8
        if (ip.StartsWith("10."))
        {
            return true;
        }
        
        // 172.16.0.0/12 (172.16.0.0 - 172.31.255.255)
        if (ip.StartsWith("172."))
        {
            var parts = ip.Split('.');
            if (parts.Length >= 2 && int.TryParse(parts[1], out var secondOctet))
            {
                if (secondOctet >= 16 && secondOctet <= 31)
                {
                    return true;
                }
            }
        }

        // IPv6 本地地址和 IPv4 回环地址
        if (ip == "::1" || ip == "127.0.0.1" || ip.StartsWith("fe80:"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 获取IP地理位置信息（使用免费的ip-api.com服务）
    /// </summary>
    private async Task<(string? Country, string? Region, string? City)> GetIpGeoLocation(string? ip)
    {
        // 修复线上访问仍然显示未知的问题：私网 IP 不解析地理位置，返回 null（前端会显示"未知"）
        if (string.IsNullOrEmpty(ip) || IsPrivateIp(ip))
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
                _logger.LogWarning("无效的时间范围: {StartDate} >= {EndDate}", startDate, endDate);
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
                        _logger.LogDebug("VisitorAnalytics 中没有找到数据");
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
                    _logger.LogError(ex, "查询 VisitorAnalytics 时出错");
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
                        _logger.LogDebug("VisitorAnalytics 中没有找到按小时的数据");
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
                    _logger.LogError(ex, "查询 VisitorAnalytics 按小时数据时出错");
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
                        _logger.LogDebug("VisitLogs 中没有找到数据");
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
                    _logger.LogError(ex, "查询 VisitLogs 时出错");
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
                    _logger.LogError(ex, "填充日期数据时出错");
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
            _logger.LogError(ex, "获取趋势数据时发生错误: Range={Range}, Granularity={Granularity}", range, granularity);

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
            _logger.LogDebug("获取访客列表: VisitorAnalytics count={Count}, onlineOnly={OnlineOnly}, page={Page}, pageSize={PageSize}", 
                analyticsCount, onlineOnly, page, pageSize);
            
            if (analyticsCount > 0)
            {
                var query = _context.VisitorAnalytics.AsQueryable();

                // 修复变量名冲突：将 fiveMinutesAgo 声明移到 if 块之前
                var fiveMinutesAgo = DateTime.Now.AddMinutes(-5);

                // 只显示在线访客（最近5分钟内有活动）
                // 注意：不依赖 IsOnline 字段，直接根据 UpdatedAt 判断，因为 IsOnline 可能没有及时更新
                if (onlineOnly)
                {
                    query = query.Where(v => v.UpdatedAt >= fiveMinutesAgo);
                }

                // 按更新时间倒序排列
                query = query.OrderByDescending(v => v.UpdatedAt);

                var total = await query.CountAsync();
                
                // 修复访客列表 IP 一直显示未知的问题：确保字段映射正确，并计算在线状态
                var visitors = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(v => new
                    {
                        v.Id,
                        v.VisitorId,
                        // 修复访客列表 IP 一直显示未知的问题：确保 IP 字段有值
                        // 如果 IP 为 null 或空，返回 "-"（前端会显示为"未知"）
                        // 如果是私网 IP，直接返回 IP 字符串（前端会显示 IP，但地理位置显示为"未知"）
                        Ip = !string.IsNullOrWhiteSpace(v.Ip) ? v.Ip : "-",
                        v.Country,
                        v.Region,
                        v.City,
                        DeviceType = v.DeviceType ?? "unknown",
                        Browser = v.Browser ?? "unknown",
                        Os = v.Os ?? "unknown",
                        Path = v.Path ?? "/", // 确保 Path 不为 null，修复"未知页面"问题
                        v.Referrer,
                        v.SearchKeyword,
                        PageViews = v.PageViews > 0 ? v.PageViews : 1, // 确保浏览量至少为 1
                        SessionStart = v.SessionStart,
                        UpdatedAt = v.UpdatedAt, // 最后活跃时间
                        IsOnline = v.UpdatedAt >= fiveMinutesAgo // 修复在线状态：最近5分钟有活动即为在线
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

                // 只显示最近5分钟的访客（在线访客）
                if (onlineOnly)
                {
                    var fiveMinutesAgo = DateTime.Now.AddMinutes(-5);
                    query = query.Where(v => v.Timestamp >= fiveMinutesAgo);
                }

                // 按时间倒序排列
                query = query.OrderByDescending(v => v.Timestamp);

                var total = await query.CountAsync();
                var visitLogs = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // 转换为访客格式
                // 修复：从 VisitLogs 表解析 IP 获取地理位置和设备信息（解决访客分析无数据问题）
                var visitorIds = visitLogs.Select(v => v.VisitorId).Distinct().ToList();
                var pageViewsDict = await _context.VisitLogs
                    .Where(log => visitorIds.Contains(log.VisitorId))
                    .GroupBy(log => log.VisitorId)
                    .Select(g => new { VisitorId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.VisitorId, x => x.Count);
                
                // 先收集需要解析的 IP（去重，避免重复解析）
                // 修复线上访问仍然显示未知的问题：只解析非私网 IP
                var ipSet = visitLogs
                    .Where(v => !string.IsNullOrEmpty(v.Ip) && !IsPrivateIp(v.Ip))
                    .Select(v => v.Ip!)
                    .Distinct()
                    .ToList();
                
                // 批量解析 IP 地理位置（使用字典缓存，避免重复解析）
                var geoCache = new Dictionary<string, (string? Country, string? Region, string? City)>();
                foreach (var ip in ipSet.Take(50)) // 限制最多解析 50 个 IP，避免性能问题
                {
                    try
                    {
                        var geoInfo = await GetIpGeoLocation(ip);
                        geoCache[ip] = geoInfo;
                    }
                    catch
                    {
                        // 静默失败，继续处理下一个 IP（修复：使用命名元组以匹配类型）
                        geoCache[ip] = (Country: null, Region: null, City: null);
                    }
                }
                
                // 构建访客列表
                // 修复访客列表 IP 一直显示未知的问题：确保 IP 字段正确映射，即使是 null 或私网 IP 也要显示
                var visitors = visitLogs.Select(v =>
                {
                    var deviceInfo = ParseDeviceInfo(v.UserAgent ?? "");
                    var pageViews = pageViewsDict.GetValueOrDefault(v.VisitorId, 1);
                    
                    // 从缓存获取地理位置信息（修复：使用明确的命名元组类型声明）
                    (string? Country, string? Region, string? City) geoInfo = !string.IsNullOrEmpty(v.Ip) && geoCache.ContainsKey(v.Ip)
                        ? geoCache[v.Ip]
                        : (Country: null, Region: null, City: null);
                    
                    // 修复访客列表 IP 一直显示未知的问题：确保字段映射正确，Path 不为空，在线状态正确计算
                    var fiveMinutesAgo = DateTime.Now.AddMinutes(-5);
                    var isOnline = v.Timestamp >= fiveMinutesAgo;
                    
                    // 修复访客列表 IP 一直显示未知的问题：确保 IP 字段正确返回
                    // 如果 IP 为 null 或空，返回 "-"（前端会显示为"未知"）
                    // 如果是私网 IP，直接返回 IP 字符串（前端会显示 IP，但地理位置显示为"未知"）
                    var ipValue = !string.IsNullOrWhiteSpace(v.Ip) ? v.Ip : "-";
                    
                    return new
                    {
                        Id = v.Id,
                        VisitorId = v.VisitorId,
                        Ip = ipValue, // 修复访客列表 IP 一直显示未知的问题：确保 IP 字段有值
                        Country = geoInfo.Country, // 修复：使用命名元组属性访问
                        Region = geoInfo.Region, // 修复：使用命名元组属性访问
                        City = geoInfo.City, // 修复：使用命名元组属性访问
                        DeviceType = deviceInfo.DeviceType ?? "unknown",
                        Browser = deviceInfo.Browser ?? "unknown",
                        Os = deviceInfo.Os ?? "unknown",
                        Path = !string.IsNullOrEmpty(v.Path) ? v.Path : "/", // 修复"未知页面"问题：确保 Path 不为空
                        Referrer = (string?)null, // VisitLogs 表没有 Referrer 字段
                        SearchKeyword = (string?)null, // VisitLogs 表没有 SearchKeyword 字段
                        PageViews = pageViews > 0 ? pageViews : 1, // 确保浏览量至少为 1
                        SessionStart = v.Timestamp,
                        UpdatedAt = v.Timestamp, // 最后活跃时间
                        IsOnline = isOnline // 修复在线状态：最近5分钟有活动即为在线
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

            // 如果 VisitorAnalytics 没有地区数据，尝试从 VisitLogs 解析 IP 获取地理位置（修复访客分析无数据问题）
            // 先统计每个 IP 的访问次数，然后解析地理位置
            var ipCounts = await _context.VisitLogs
                .Where(v => v.Timestamp >= startDate && v.Timestamp < endDate && !string.IsNullOrEmpty(v.Ip))
                .GroupBy(v => v.Ip)
                .Select(g => new { Ip = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(100) // 限制最多解析 100 个 IP，避免性能问题
                .ToListAsync();
            
            var regionDict = new Dictionary<string, int>(); // key: "country|region", value: count
            
            // 批量解析 IP 地理位置（按访问次数排序，优先解析访问量大的 IP）
            // 修复线上访问仍然显示未知的问题：只解析非私网 IP
            foreach (var ipCount in ipCounts)
            {
                var ip = ipCount.Ip;
                if (string.IsNullOrEmpty(ip) || IsPrivateIp(ip))
                {
                    continue; // 跳过私网 IP
                }
                
                try
                {
                    var geoInfo = await GetIpGeoLocation(ip);
                    if (!string.IsNullOrEmpty(geoInfo.Country))
                    {
                        var key = $"{geoInfo.Country}|{geoInfo.Region ?? ""}";
                        regionDict[key] = regionDict.GetValueOrDefault(key, 0) + ipCount.Count;
                    }
                }
                catch
                {
                    // 静默失败，继续处理下一个 IP
                }
            }
            
            if (regionDict.Count > 0)
            {
                var items = regionDict.Select(kvp =>
                {
                    var parts = kvp.Key.Split('|');
                    return new
                    {
                        country = parts[0],
                        province = parts.Length > 1 ? parts[1] : "",
                        count = kvp.Value
                    };
                }).OrderByDescending(x => x.count).ToList();
                
                return Ok(ApiResponse.Success(new { items }));
            }
            
            // 如果仍然没有数据，返回空列表
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

