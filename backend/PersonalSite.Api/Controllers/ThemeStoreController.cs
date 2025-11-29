using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Services.Payment;
using System.Text.Json;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 主题商店控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ThemeStoreController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ThemeStoreController> _logger;
    private readonly PaymentServiceFactory _paymentServiceFactory;

    public ThemeStoreController(
        AppDbContext context,
        ILogger<ThemeStoreController> logger,
        PaymentServiceFactory paymentServiceFactory)
    {
        _context = context;
        _logger = logger;
        _paymentServiceFactory = paymentServiceFactory;
    }

    /// <summary>
    /// 获取主题列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetThemes(
        [FromQuery] string? category = null,
        [FromQuery] bool? isFree = null,
        [FromQuery] string? sortBy = "popular", // popular, price, rating, newest
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        try
        {
            var query = _context.ThemeStores.Where(t => t.Status == "published");

            // 分类筛选
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(t => t.Category == category);
            }

            // 免费/付费筛选
            if (isFree.HasValue)
            {
                query = query.Where(t => t.IsFree == isFree.Value);
            }

            // 排序
            query = sortBy switch
            {
                "price" => query.OrderBy(t => t.Price),
                "price_desc" => query.OrderByDescending(t => t.Price),
                "rating" => query.OrderByDescending(t => t.Rating),
                "newest" => query.OrderByDescending(t => t.CreatedAt),
                _ => query.OrderByDescending(t => t.DownloadCount + t.PurchaseCount)
            };

            var total = await query.CountAsync();
            // 先查询出基础数据，避免在表达式树中使用可选参数
            var themesData = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Slug,
                    t.Description,
                    t.PreviewImage,
                    t.Price,
                    t.IsFree,
                    t.Category,
                    t.DownloadCount,
                    t.PurchaseCount,
                    t.Rating,
                    t.RatingCount,
                    t.AuthorName,
                    TagsJson = t.Tags // 先获取 JSON 字符串
                })
                .ToListAsync();

            // 在内存中反序列化 JSON 数据
            var themes = themesData.Select(t => new
            {
                t.Id,
                t.Name,
                t.Slug,
                t.Description,
                t.PreviewImage,
                t.Price,
                t.IsFree,
                t.Category,
                t.DownloadCount,
                t.PurchaseCount,
                t.Rating,
                t.RatingCount,
                t.AuthorName,
                Tags = !string.IsNullOrEmpty(t.TagsJson) 
                    ? JsonSerializer.Deserialize<string[]>(t.TagsJson) ?? Array.Empty<string>()
                    : Array.Empty<string>()
            }).ToList();

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
    /// 获取主题详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetTheme(long id)
    {
        try
        {
            var theme = await _context.ThemeStores.FindAsync(id);
            if (theme == null)
            {
                return NotFound(ApiResponse.Error("主题不存在", 404));
            }

            // 检查用户是否已购买
            var userId = Request.Headers["X-User-Id"].ToString();
            bool isPurchased = false;
            if (!string.IsNullOrEmpty(userId))
            {
                isPurchased = await _context.ThemePurchases
                    .AnyAsync(p => p.UserId == userId && p.ThemeId == id && p.IsActive && p.PaymentStatus == "paid");
            }

            var result = new
            {
                theme.Id,
                theme.Name,
                theme.Slug,
                theme.Description,
                theme.PreviewImage,
                theme.PreviewImages,
                theme.Price,
                theme.IsFree,
                theme.Category,
                theme.DownloadCount,
                theme.PurchaseCount,
                theme.Rating,
                theme.RatingCount,
                theme.AuthorName,
                theme.Features,
                theme.Version,
                IsPurchased = isPurchased || theme.IsFree,
                Tags = !string.IsNullOrEmpty(theme.Tags) 
                    ? JsonSerializer.Deserialize<string[]>(theme.Tags) ?? Array.Empty<string>()
                    : Array.Empty<string>()
            };

            return Ok(ApiResponse.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取主题详情失败");
            return StatusCode(500, ApiResponse.Error($"获取主题详情失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 购买主题
    /// </summary>
    [HttpPost("purchase")]
    public async Task<ActionResult<ApiResponse<object>>> PurchaseTheme([FromBody] ThemePurchaseRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest(ApiResponse.Error("UserId is required", 400));
            }

            var theme = await _context.ThemeStores.FindAsync(request.ThemeId);
            if (theme == null)
            {
                return NotFound(ApiResponse.Error("主题不存在", 404));
            }

            // 免费主题直接返回
            if (theme.IsFree)
            {
                return Ok(ApiResponse.Success(new
                {
                    Message = "免费主题，可直接使用",
                    ThemeId = theme.Id
                }));
            }

            // 检查是否已购买
            var existingPurchase = await _context.ThemePurchases
                .FirstOrDefaultAsync(p => 
                    p.UserId == request.UserId && 
                    p.ThemeId == request.ThemeId && 
                    p.IsActive && 
                    p.PaymentStatus == "paid");

            if (existingPurchase != null)
            {
                return Ok(ApiResponse.Success(new
                {
                    Message = "您已拥有此主题",
                    PurchaseId = existingPurchase.Id
                }));
            }

            // 创建购买记录
            var purchase = new ThemePurchase
            {
                UserId = request.UserId,
                ThemeId = request.ThemeId,
                PurchaseType = request.PurchaseType ?? "one_time",
                Price = theme.Price,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = "pending", // 待支付
                PaymentId = request.PaymentId,
                IsActive = false
            };

            _context.ThemePurchases.Add(purchase);
            await _context.SaveChangesAsync();

            // 创建支付订单
            var paymentService = _paymentServiceFactory.GetPaymentService(request.PaymentMethod ?? "wechat");
            var paymentRequest = new PersonalSite.Api.Services.Payment.PaymentRequest
            {
                OrderId = purchase.Id.ToString(),
                Amount = (int)(purchase.Price * 100), // 转换为分
                Description = $"购买主题: {theme.Name}",
                UserId = request.UserId,
                PaymentMethod = request.PaymentMethod ?? "wechat",
                ClientIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                NotifyUrl = $"{Request.Scheme}://{Request.Host}/api/payment/callback/{request.PaymentMethod ?? "wechat"}",
                ReturnUrl = $"{Request.Scheme}://{Request.Host}/payment/success",
                ExtraData = new Dictionary<string, string>
                {
                    ["purchase_id"] = purchase.Id.ToString(),
                    ["theme_id"] = theme.Id.ToString(),
                    ["type"] = "theme"
                }
            };

            var paymentResponse = await paymentService.CreatePaymentAsync(paymentRequest);

            if (!paymentResponse.Success)
            {
                return BadRequest(ApiResponse.Error(paymentResponse.ErrorMessage ?? "创建支付订单失败", 400));
            }

            // 更新购买记录的支付ID
            purchase.PaymentId = paymentResponse.PaymentId;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new
            {
                PurchaseId = purchase.Id,
                PaymentId = paymentResponse.PaymentId,
                PaymentUrl = paymentResponse.PaymentUrl,
                QrCode = paymentResponse.QrCode,
                PaymentParams = paymentResponse.PaymentParams,
                Message = "请完成支付",
                PaymentStatus = purchase.PaymentStatus,
                Price = purchase.Price
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "购买主题失败");
            return StatusCode(500, ApiResponse.Error($"购买主题失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 应用主题
    /// </summary>
    [HttpPost("apply")]
    public async Task<ActionResult<ApiResponse<object>>> ApplyTheme([FromBody] ThemeApplyRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest(ApiResponse.Error("UserId is required", 400));
            }

            var theme = await _context.ThemeStores.FindAsync(request.ThemeId);
            if (theme == null)
            {
                return NotFound(ApiResponse.Error("主题不存在", 404));
            }

            // 检查权限（免费或已购买）
            if (!theme.IsFree)
            {
                var hasPurchase = await _context.ThemePurchases
                    .AnyAsync(p => 
                        p.UserId == request.UserId && 
                        p.ThemeId == request.ThemeId && 
                        p.IsActive && 
                        p.PaymentStatus == "paid");

                if (!hasPurchase)
                {
                    return BadRequest(ApiResponse.Error("您尚未购买此主题", 400));
                }
            }

            // TODO: 保存用户主题偏好
            // 可以存储在用户设置表中

            // 更新主题下载/使用次数
            theme.DownloadCount++;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new
            {
                Message = "主题应用成功",
                ThemeId = theme.Id,
                ThemeName = theme.Name
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "应用主题失败");
            return StatusCode(500, ApiResponse.Error($"应用主题失败: {ex.Message}", 500));
        }
    }
}

// 请求模型
public class ThemePurchaseRequest
{
    public long ThemeId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? PurchaseType { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentId { get; set; }
}

public class ThemeApplyRequest
{
    public long ThemeId { get; set; }
    public string UserId { get; set; } = string.Empty;
}

