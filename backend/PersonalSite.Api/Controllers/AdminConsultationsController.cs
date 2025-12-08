using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 后台咨询管理控制器
/// </summary>
[ApiController]
[Route("api/admin/consultations")]
[Authorize]
public class AdminConsultationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<AdminConsultationsController> _logger;

    public AdminConsultationsController(AppDbContext context, ILogger<AdminConsultationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 获取咨询列表（分页、筛选）
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetConsultations(
        [FromQuery] int? status = null,
        [FromQuery] string? keyword = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = _context.PreSaleConsultations.AsQueryable();

            // 状态筛选
            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            // 关键词搜索（客户名、联系方式、商品名）
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(c =>
                    c.CustomerName.Contains(keyword) ||
                    c.ProductNameSnapshot.Contains(keyword) ||
                    (c.CustomerPhone != null && c.CustomerPhone.Contains(keyword)) ||
                    (c.CustomerEmail != null && c.CustomerEmail.Contains(keyword)) ||
                    (c.CustomerWeChat != null && c.CustomerWeChat.Contains(keyword)));
            }

            var total = await query.CountAsync();
            var consultations = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.Id,
                    c.ProductId,
                    c.ProductNameSnapshot,
                    c.CustomerName,
                    c.CustomerPhone,
                    c.CustomerWeChat,
                    c.CustomerEmail,
                    c.BudgetRange,
                    c.ExpectedDeadline,
                    c.Status,
                    c.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse.Success(new
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                List = consultations
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取咨询列表失败");
            return StatusCode(500, ApiResponse.Error($"获取咨询列表失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取咨询详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetConsultationDetail(long id)
    {
        try
        {
            var consultation = await _context.PreSaleConsultations.FindAsync(id);
            if (consultation == null)
            {
                return NotFound(ApiResponse.Error("咨询不存在", 404));
            }

            return Ok(ApiResponse.Success(new
            {
                consultation.Id,
                consultation.ProductId,
                consultation.ProductNameSnapshot,
                consultation.CustomerName,
                consultation.CustomerPhone,
                consultation.CustomerWeChat,
                consultation.CustomerEmail,
                consultation.BudgetRange,
                consultation.ExpectedDeadline,
                consultation.RequirementDescription,
                consultation.Status,
                consultation.InternalNote,
                consultation.CreatedAt,
                consultation.UpdatedAt
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取咨询详情失败");
            return StatusCode(500, ApiResponse.Error($"获取咨询详情失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 更新咨询状态和内部备注
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse>> UpdateConsultation(long id, [FromBody] UpdateConsultationRequest request)
    {
        try
        {
            var consultation = await _context.PreSaleConsultations.FindAsync(id);
            if (consultation == null)
            {
                return NotFound(ApiResponse.Error("咨询不存在", 404));
            }

            // 更新状态
            consultation.Status = request.Status;

            // 更新内部备注（如果提供了新备注，则追加或覆盖）
            if (!string.IsNullOrWhiteSpace(request.InternalNote))
            {
                if (string.IsNullOrWhiteSpace(consultation.InternalNote))
                {
                    consultation.InternalNote = request.InternalNote;
                }
                else
                {
                    // 追加新备注（带时间戳）
                    consultation.InternalNote = $"{consultation.InternalNote}\n\n[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {request.InternalNote}";
                }
            }

            consultation.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新咨询失败");
            return StatusCode(500, ApiResponse.Error($"更新咨询失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 将咨询转为订单
    /// </summary>
    [HttpPost("{id}/convert-to-order")]
    public async Task<ActionResult<ApiResponse<ConvertToOrderResponse>>> ConvertToOrder(long id)
    {
        try
        {
            var consultation = await _context.PreSaleConsultations.FindAsync(id);
            if (consultation == null)
            {
                return NotFound(ApiResponse<ConvertToOrderResponse>.Error("咨询不存在", 404));
            }

            // 生成订单编号
            var orderNo = GenerateOrderNo();

            // 创建订单
            var order = new Order
            {
                OrderNo = orderNo,
                ProductId = consultation.ProductId,
                ProductNameSnapshot = consultation.ProductNameSnapshot,
                PriceSnapshot = null, // 咨询转订单时价格通常需要面议
                Quantity = 1,
                TotalAmount = null,
                CustomerName = consultation.CustomerName,
                CustomerPhone = consultation.CustomerPhone,
                CustomerWeChat = consultation.CustomerWeChat,
                CustomerEmail = consultation.CustomerEmail,
                Remark = consultation.RequirementDescription, // 将需求描述填入备注
                Status = (int)OrderStatus.PendingReview,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Orders.Add(order);

            // 更新咨询状态为"已转为订单"
            consultation.Status = (int)ConsultationStatus.ConvertedToOrder;
            consultation.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<ConvertToOrderResponse>.Success(new ConvertToOrderResponse
            {
                OrderId = order.Id,
                OrderNo = order.OrderNo
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "转换咨询为订单失败");
            return StatusCode(500, ApiResponse<ConvertToOrderResponse>.Error($"转换咨询为订单失败: {ex.Message}", 500));
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
/// 更新咨询请求 DTO
/// </summary>
public class UpdateConsultationRequest
{
    /// <summary>
    /// 咨询状态（0-新咨询, 1-已联系, 2-已转为订单, 3-已关闭/无意向）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 内部备注
    /// </summary>
    public string? InternalNote { get; set; }
}

/// <summary>
/// 转换咨询为订单响应 DTO
/// </summary>
public class ConvertToOrderResponse
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

