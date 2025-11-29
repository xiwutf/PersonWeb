using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers.Admin;

/// <summary>
/// 商业化功能管理控制器
/// </summary>
[ApiController]
[Route("api/admin/commercial")]
[Authorize] // 需要管理员权限
public class CommercialAdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<CommercialAdminController> _logger;

    public CommercialAdminController(AppDbContext context, ILogger<CommercialAdminController> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region 主题商店管理

    /// <summary>
    /// 获取主题列表（管理）
    /// </summary>
    [HttpGet("themes")]
    public async Task<ActionResult<ApiResponse<object>>> GetThemes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        try
        {
            var query = _context.ThemeStores.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            var total = await query.CountAsync();
            var themes = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Slug,
                    t.Description,
                    t.Price,
                    t.IsFree,
                    t.Status,
                    t.DownloadCount,
                    t.PurchaseCount,
                    t.Rating,
                    t.RatingCount,
                    t.CreatedAt,
                    t.UpdatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse.Success(new
            {
                Themes = themes,
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取主题列表失败");
            return StatusCode(500, ApiResponse.Error($"获取主题列表失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取主题购买记录
    /// </summary>
    [HttpGet("theme-purchases")]
    public async Task<ActionResult<ApiResponse<object>>> GetThemePurchases(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        try
        {
            var query = _context.ThemePurchases
                .Include(p => p.Theme)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(p => p.PaymentStatus == status);
            }

            var total = await query.CountAsync();
            var purchases = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id,
                    p.UserId,
                    Theme = p.Theme != null ? new { p.Theme.Id, p.Theme.Name } : null,
                    p.Price,
                    p.PaymentMethod,
                    p.PaymentStatus,
                    p.PaidAt,
                    p.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse.Success(new
            {
                Purchases = purchases,
                Total = total,
                Page = page,
                PageSize = pageSize
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取主题购买记录失败");
            return StatusCode(500, ApiResponse.Error($"获取主题购买记录失败: {ex.Message}", 500));
        }
    }

    #endregion

    #region API Key 管理

    /// <summary>
    /// 获取 API 用户列表
    /// </summary>
    [HttpGet("api-users")]
    public async Task<ActionResult<ApiResponse<object>>> GetApiUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var total = await _context.ApiUsers.CountAsync();
            var users = await _context.ApiUsers
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new
                {
                    u.Id,
                    UserId = u.Id, // ApiUser 使用 Id 作为用户标识
                    u.Email,
                    u.Name,
                    u.FreeCallsUsed,
                    u.PaidCalls,
                    u.LastCallAt,
                    u.CreatedAt,
                    ApiKeyCount = u.ApiKeys.Count(k => k.IsActive)
                })
                .ToListAsync();

            return Ok(ApiResponse.Success(new
            {
                Users = users,
                Total = total,
                Page = page,
                PageSize = pageSize
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取API用户列表失败");
            return StatusCode(500, ApiResponse.Error($"获取API用户列表失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取 API 调用统计
    /// </summary>
    [HttpGet("api-calls/stats")]
    public async Task<ActionResult<ApiResponse<object>>> GetApiCallStats(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var query = _context.ApiCalls.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(c => c.CalledAt >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(c => c.CalledAt <= endDate.Value);
            }

            var stats = await query
                .GroupBy(c => c.CalledAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count(),
                    TotalCost = g.Sum(c => c.Cost),
                    AvgResponseTime = g.Average(c => c.ResponseTime ?? 0)
                })
                .OrderBy(s => s.Date)
                .ToListAsync();

            var totalStats = new
            {
                TotalCalls = await query.CountAsync(),
                TotalCost = await query.SumAsync(c => c.Cost),
                AvgResponseTime = await query.AverageAsync(c => c.ResponseTime ?? 0)
            };

            return Ok(ApiResponse.Success(new
            {
                Daily = stats,
                Total = totalStats
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取API调用统计失败");
            return StatusCode(500, ApiResponse.Error($"获取API调用统计失败: {ex.Message}", 500));
        }
    }

    #endregion

    #region 会员管理

    /// <summary>
    /// 获取会员列表
    /// </summary>
    [HttpGet("memberships")]
    public async Task<ActionResult<ApiResponse<object>>> GetMemberships(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        try
        {
            var query = _context.UserMemberships
                .Include(m => m.MembershipType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(m => m.Status == status);
            }

            var total = await query.CountAsync();
            var memberships = await query
                .OrderByDescending(m => m.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.Id,
                    m.UserId,
                    MembershipType = m.MembershipType != null ? new { m.MembershipType.Name, m.MembershipType.Price } : null,
                    m.StartDate,
                    m.EndDate,
                    m.Status,
                    m.AutoRenew,
                    m.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse.Success(new
            {
                Memberships = memberships,
                Total = total,
                Page = page,
                PageSize = pageSize
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取会员列表失败");
            return StatusCode(500, ApiResponse.Error($"获取会员列表失败: {ex.Message}", 500));
        }
    }

    #endregion

    #region 支付订单管理

    /// <summary>
    /// 获取支付订单列表
    /// </summary>
    [HttpGet("orders")]
    public async Task<ActionResult<ApiResponse<object>>> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? paymentMethod = null)
    {
        try
        {
            // 合并所有类型的订单
            var themePurchases = await _context.ThemePurchases
                .Where(p => string.IsNullOrEmpty(status) || p.PaymentStatus == status)
                .Where(p => string.IsNullOrEmpty(paymentMethod) || p.PaymentMethod == paymentMethod)
                .Select(p => new
                {
                    Id = p.Id,
                    OrderId = p.Id.ToString(),
                    Type = "theme",
                    UserId = p.UserId ?? string.Empty,
                    Amount = p.Price,
                    PaymentMethod = p.PaymentMethod ?? string.Empty,
                    PaymentStatus = p.PaymentStatus,
                    CreatedAt = p.CreatedAt,
                    PaidAt = p.PaidAt
                })
                .ToListAsync();

            var toolPurchases = await _context.ToolPurchases
                .Where(p => string.IsNullOrEmpty(status) || p.PaymentStatus == status)
                .Where(p => string.IsNullOrEmpty(paymentMethod) || p.PaymentMethod == paymentMethod)
                .Select(p => new
                {
                    Id = p.Id,
                    OrderId = p.Id.ToString(),
                    Type = "tool",
                    UserId = p.UserId ?? string.Empty,
                    Amount = p.Price,
                    PaymentMethod = p.PaymentMethod ?? string.Empty,
                    PaymentStatus = p.PaymentStatus,
                    CreatedAt = p.CreatedAt,
                    PaidAt = p.PaidAt
                })
                .ToListAsync();

            var allOrders = themePurchases.Concat(toolPurchases)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var total = themePurchases.Count + toolPurchases.Count;

            return Ok(ApiResponse.Success(new
            {
                Orders = allOrders,
                Total = total,
                Page = page,
                PageSize = pageSize
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取支付订单列表失败");
            return StatusCode(500, ApiResponse.Error($"获取支付订单列表失败: {ex.Message}", 500));
        }
    }

    #endregion
}

