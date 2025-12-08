using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            // 校验：需求描述必填
            if (string.IsNullOrWhiteSpace(request.RequirementDescription))
            {
                return BadRequest(ApiResponse<CreateConsultationResponse>.Error("需求描述不能为空", 400));
            }

            // 校验：至少有一种联系方式非空
            if (string.IsNullOrWhiteSpace(request.CustomerPhone) &&
                string.IsNullOrWhiteSpace(request.CustomerWeChat) &&
                string.IsNullOrWhiteSpace(request.CustomerEmail))
            {
                return BadRequest(ApiResponse<CreateConsultationResponse>.Error("至少需要填写一种联系方式（手机号、微信号或邮箱）", 400));
            }

            // 校验：客户姓名必填
            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                return BadRequest(ApiResponse<CreateConsultationResponse>.Error("客户姓名不能为空", 400));
            }

            // 查询商品信息
            var product = await _context.Tools.FindAsync(request.ProductId);
            if (product == null)
            {
                return NotFound(ApiResponse<CreateConsultationResponse>.Error("商品不存在", 404));
            }

            // 创建咨询记录
            var consultation = new PreSaleConsultation
            {
                ProductId = request.ProductId,
                ProductNameSnapshot = product.Name,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                CustomerWeChat = request.CustomerWeChat,
                CustomerEmail = request.CustomerEmail,
                BudgetRange = request.BudgetRange,
                ExpectedDeadline = request.ExpectedDeadline,
                RequirementDescription = request.RequirementDescription,
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

