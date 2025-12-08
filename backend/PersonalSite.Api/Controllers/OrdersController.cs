using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 订单控制器（前台接口）
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(AppDbContext context, ILogger<OrdersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 创建订单（匿名接口）
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<CreateOrderResponse>>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            // 校验：至少有一种联系方式非空
            if (string.IsNullOrWhiteSpace(request.CustomerPhone) &&
                string.IsNullOrWhiteSpace(request.CustomerWeChat) &&
                string.IsNullOrWhiteSpace(request.CustomerEmail))
            {
                return BadRequest(ApiResponse<CreateOrderResponse>.Error("至少需要填写一种联系方式（手机号、微信号或邮箱）", 400));
            }

            // 校验：客户姓名必填
            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                return BadRequest(ApiResponse<CreateOrderResponse>.Error("客户姓名不能为空", 400));
            }

            // 查询商品信息
            var product = await _context.Tools.FindAsync(request.ProductId);
            if (product == null)
            {
                return NotFound(ApiResponse<CreateOrderResponse>.Error("商品不存在", 404));
            }

            // 生成订单编号（格式：日期-随机数，如：20251208-XXXX）
            var orderNo = GenerateOrderNo();

            // 计算总金额
            decimal? totalAmount = null;
            if (request.Quantity > 0 && product.Price > 0)
            {
                totalAmount = product.Price * request.Quantity;
            }

            // 创建订单
            var order = new Order
            {
                OrderNo = orderNo,
                ProductId = request.ProductId,
                ProductNameSnapshot = product.Name,
                PriceSnapshot = product.Price > 0 ? product.Price : null,
                Quantity = request.Quantity > 0 ? request.Quantity : 1,
                TotalAmount = totalAmount,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                CustomerWeChat = request.CustomerWeChat,
                CustomerEmail = request.CustomerEmail,
                Remark = request.Remark,
                Status = (int)OrderStatus.PendingReview,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<CreateOrderResponse>.Success(new CreateOrderResponse
            {
                OrderId = order.Id,
                OrderNo = order.OrderNo
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建订单失败");
            return StatusCode(500, ApiResponse<CreateOrderResponse>.Error($"创建订单失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 订单查询（匿名接口，通过订单号和联系方式查询）
    /// </summary>
    [HttpGet("lookup")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<OrderLookupResponse>>> LookupOrder(
        [FromQuery] string orderNo,
        [FromQuery] string contact)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(orderNo) || string.IsNullOrWhiteSpace(contact))
            {
                return BadRequest(ApiResponse<OrderLookupResponse>.Error("订单号和联系方式不能为空", 400));
            }

            // 查询订单
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderNo == orderNo);

            if (order == null)
            {
                return NotFound(ApiResponse<OrderLookupResponse>.Error("订单不存在", 404));
            }

            // 验证联系方式是否匹配
            var contactMatched = 
                (!string.IsNullOrWhiteSpace(order.CustomerPhone) && order.CustomerPhone.Contains(contact)) ||
                (!string.IsNullOrWhiteSpace(order.CustomerEmail) && order.CustomerEmail.Contains(contact)) ||
                (!string.IsNullOrWhiteSpace(order.CustomerWeChat) && order.CustomerWeChat.Contains(contact));

            if (!contactMatched)
            {
                return NotFound(ApiResponse<OrderLookupResponse>.Error("订单不存在或联系方式不匹配", 404));
            }

            // 返回订单信息
            var response = new OrderLookupResponse
            {
                OrderNo = order.OrderNo,
                ProductName = order.ProductNameSnapshot,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                Remark = order.Remark
            };

            return Ok(ApiResponse<OrderLookupResponse>.Success(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询订单失败");
            return StatusCode(500, ApiResponse<OrderLookupResponse>.Error($"查询订单失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 生成订单编号（格式：日期-随机数，如：20251208-XXXX）
    /// </summary>
    private string GenerateOrderNo()
    {
        var dateStr = DateTime.Now.ToString("yyyyMMdd");
        var random = new Random();
        var randomStr = random.Next(1000, 9999).ToString();
        return $"{dateStr}-{randomStr}";
    }
}

/// <summary>
/// 创建订单请求 DTO
/// </summary>
public class CreateOrderRequest
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public long ProductId { get; set; }

    /// <summary>
    /// 客户姓名
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 客户手机号
    /// </summary>
    public string? CustomerPhone { get; set; }

    /// <summary>
    /// 客户微信号
    /// </summary>
    public string? CustomerWeChat { get; set; }

    /// <summary>
    /// 客户邮箱
    /// </summary>
    public string? CustomerEmail { get; set; }

    /// <summary>
    /// 需求说明/备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 数量（默认1）
    /// </summary>
    public int Quantity { get; set; } = 1;
}

/// <summary>
/// 创建订单响应 DTO
/// </summary>
public class CreateOrderResponse
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public long OrderId { get; set; }

    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;
}

/// <summary>
/// 订单查询响应 DTO
/// </summary>
public class OrderLookupResponse
{
    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 订单状态（0-待确认, 1-进行中, 2-已完成, 3-已关闭）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 备注/需求说明
    /// </summary>
    public string? Remark { get; set; }
}

