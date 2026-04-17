using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 购买前咨询控制器（前台接口）
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConsultationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ConsultationsController> _logger;

    public ConsultationsController(AppDbContext context, ILogger<ConsultationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 创建咨询（匿名接口）
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<CreateConsultationResponse>>> CreateConsultation([FromBody] CreateConsultationRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.RequirementDescription))
            {
                return BadRequest(ApiResponse<CreateConsultationResponse>.Error("需求描述不能为空", 400));
            }

            if (string.IsNullOrWhiteSpace(request.CustomerPhone) &&
                string.IsNullOrWhiteSpace(request.CustomerWeChat) &&
                string.IsNullOrWhiteSpace(request.CustomerEmail))
            {
                return BadRequest(ApiResponse<CreateConsultationResponse>.Error("至少需要填写一种联系方式（手机号、微信号或邮箱）", 400));
            }

            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                return BadRequest(ApiResponse<CreateConsultationResponse>.Error("客户姓名不能为空", 400));
            }

            Tool? product = null;
            var productId = request.ProductId > 0 ? request.ProductId : 0;
            var productNameSnapshot = "官网咨询";

            // productId > 0 保持原有商品咨询逻辑；productId <= 0 允许首页快速咨询等场景
            if (productId > 0)
            {
                product = await _context.Tools.FindAsync(productId);
                if (product == null)
                {
                    return NotFound(ApiResponse<CreateConsultationResponse>.Error("商品不存在", 404));
                }

                productNameSnapshot = product.Name;
            }

            var consultation = new PreSaleConsultation
            {
                ProductId = productId,
                ProductNameSnapshot = productNameSnapshot,
                CustomerName = request.CustomerName.Trim(),
                CustomerPhone = request.CustomerPhone?.Trim(),
                CustomerWeChat = request.CustomerWeChat?.Trim(),
                CustomerEmail = request.CustomerEmail?.Trim(),
                BudgetRange = request.BudgetRange?.Trim(),
                ExpectedDeadline = request.ExpectedDeadline?.Trim(),
                RequirementDescription = request.RequirementDescription.Trim(),
                Status = (int)ConsultationStatus.New,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.PreSaleConsultations.Add(consultation);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<CreateConsultationResponse>.Success(new CreateConsultationResponse
            {
                ConsultationId = consultation.Id
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建咨询失败");
            return StatusCode(500, ApiResponse<CreateConsultationResponse>.Error($"创建咨询失败: {ex.Message}", 500));
        }
    }
}

/// <summary>
/// 创建咨询请求 DTO
/// </summary>
public class CreateConsultationRequest
{
    /// <summary>
    /// 商品ID，首页快速咨询可传 0
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
    /// 预算范围（如：<500, 500-1000, 1000-3000, 待评估）
    /// </summary>
    public string? BudgetRange { get; set; }

    /// <summary>
    /// 期望完成时间（如：3天内, 一周内, 两周内, 不着急）
    /// </summary>
    public string? ExpectedDeadline { get; set; }

    /// <summary>
    /// 需求描述（必填）
    /// </summary>
    public string RequirementDescription { get; set; } = string.Empty;
}

/// <summary>
/// 创建咨询响应 DTO
/// </summary>
public class CreateConsultationResponse
{
    /// <summary>
    /// 咨询ID
    /// </summary>
    public long ConsultationId { get; set; }
}
