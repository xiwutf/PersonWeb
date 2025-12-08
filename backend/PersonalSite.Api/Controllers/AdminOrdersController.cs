using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 后台订单管理控制器
/// </summary>
[ApiController]
[Route("api/admin/orders")]
[Authorize]
public class AdminOrdersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<AdminOrdersController> _logger;

    public AdminOrdersController(AppDbContext context, ILogger<AdminOrdersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 获取订单列表（分页、筛选）
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetOrders(
        [FromQuery] int? status = null,
        [FromQuery] string? keyword = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = _context.Orders.AsQueryable();

            // 状态筛选
            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            // 关键词搜索（订单号、客户名、电话、邮箱、微信）
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(o =>
                    o.OrderNo.Contains(keyword) ||
                    o.CustomerName.Contains(keyword) ||
                    (o.CustomerPhone != null && o.CustomerPhone.Contains(keyword)) ||
                    (o.CustomerEmail != null && o.CustomerEmail.Contains(keyword)) ||
                    (o.CustomerWeChat != null && o.CustomerWeChat.Contains(keyword)));
            }

            var total = await query.CountAsync();
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new
                {
                    o.Id,
                    o.OrderNo,
                    o.ProductNameSnapshot,
                    o.CustomerName,
                    o.CustomerPhone,
                    o.CustomerWeChat,
                    o.CustomerEmail,
                    o.Status,
                    o.TotalAmount,
                    o.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse.Success(new
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                List = orders
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取订单列表失败");
            return StatusCode(500, ApiResponse.Error($"获取订单列表失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取订单详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetOrderDetail(long id)
    {
        try
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(ApiResponse.Error("订单不存在", 404));
            }

            return Ok(ApiResponse.Success(new
            {
                order.Id,
                order.OrderNo,
                order.ProductId,
                order.ProductNameSnapshot,
                order.PriceSnapshot,
                order.Quantity,
                order.TotalAmount,
                order.CustomerName,
                order.CustomerPhone,
                order.CustomerWeChat,
                order.CustomerEmail,
                order.Remark,
                order.Status,
                order.InternalNote,
                order.CreatedAt,
                order.UpdatedAt
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取订单详情失败");
            return StatusCode(500, ApiResponse.Error($"获取订单详情失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 更新订单状态和内部备注
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ActionResult<ApiResponse>> UpdateOrderStatus(long id, [FromBody] UpdateOrderStatusRequest request)
    {
        try
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(ApiResponse.Error("订单不存在", 404));
            }

            // 更新状态
            order.Status = request.Status;
            
            // 更新内部备注（如果提供了新备注，则追加或覆盖）
            if (!string.IsNullOrWhiteSpace(request.InternalNote))
            {
                if (string.IsNullOrWhiteSpace(order.InternalNote))
                {
                    order.InternalNote = request.InternalNote;
                }
                else
                {
                    // 追加新备注（带时间戳）
                    order.InternalNote = $"{order.InternalNote}\n\n[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {request.InternalNote}";
                }
            }

            order.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新订单状态失败");
            return StatusCode(500, ApiResponse.Error($"更新订单状态失败: {ex.Message}", 500));
        }
    }
}

/// <summary>
/// 更新订单状态请求 DTO
/// </summary>
public class UpdateOrderStatusRequest
{
    /// <summary>
    /// 订单状态（0-待确认, 1-进行中, 2-已完成, 3-已关闭）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 内部备注
    /// </summary>
    public string? InternalNote { get; set; }
}

