using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Text.Json;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 会员系统控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MembershipController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<MembershipController> _logger;

    public MembershipController(AppDbContext context, ILogger<MembershipController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 获取会员类型列表
    /// </summary>
    [HttpGet("types")]
    public async Task<ActionResult<ApiResponse<object>>> GetMembershipTypes()
    {
        try
        {
            var types = await _context.MembershipTypes
                .Where(t => t.IsActive)
                .OrderBy(t => t.SortOrder)
                .ThenBy(t => t.Price)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Code,
                    t.Price,
                    t.DurationDays,
                    Features = !string.IsNullOrEmpty(t.Features) 
                        ? JsonSerializer.Deserialize<string[]>(t.Features) ?? Array.Empty<string>()
                        : Array.Empty<string>(),
                    t.Description
                })
                .ToListAsync();

            return Ok(ApiResponse.Success(types));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取会员类型失败");
            return StatusCode(500, ApiResponse.Error($"获取会员类型失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 购买会员
    /// </summary>
    [HttpPost("purchase")]
    public async Task<ActionResult<ApiResponse<object>>> PurchaseMembership([FromBody] MembershipPurchaseRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest(ApiResponse.Error("UserId is required", 400));
            }

            var membershipType = await _context.MembershipTypes.FindAsync(request.MembershipTypeId);
            if (membershipType == null || !membershipType.IsActive)
            {
                return NotFound(ApiResponse.Error("会员类型不存在", 404));
            }

            // 计算结束日期
            DateTime? endDate = null;
            if (membershipType.DurationDays.HasValue)
            {
                endDate = DateTime.Now.AddDays(membershipType.DurationDays.Value);
            }

            // 创建会员记录
            var membership = new UserMembership
            {
                UserId = request.UserId,
                MembershipTypeId = request.MembershipTypeId,
                StartDate = DateTime.Now,
                EndDate = endDate,
                Status = "active",
                AutoRenew = request.AutoRenew ?? false
            };

            _context.UserMemberships.Add(membership);
            await _context.SaveChangesAsync();

            // TODO: 集成支付系统

            return Ok(ApiResponse.Success(new
            {
                MembershipId = membership.Id,
                StartDate = membership.StartDate,
                EndDate = membership.EndDate,
                Message = "会员购买成功，请完成支付"
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "购买会员失败");
            return StatusCode(500, ApiResponse.Error($"购买会员失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取用户会员信息
    /// </summary>
    [HttpPost("info")]
    public async Task<ActionResult<ApiResponse<object>>> GetUserMembership([FromBody] UserIdRequest request)
    {
        try
        {
            var membership = await _context.UserMemberships
                .Include(m => m.MembershipType)
                .Where(m => m.UserId == request.UserId && m.Status == "active")
                .OrderByDescending(m => m.StartDate)
                .FirstOrDefaultAsync();

            if (membership == null)
            {
                return Ok(ApiResponse.Success(new
                {
                    HasMembership = false,
                    Message = "您还不是会员"
                }));
            }

            // 检查是否过期
            bool isExpired = membership.EndDate.HasValue && membership.EndDate.Value < DateTime.Now;
            if (isExpired)
            {
                membership.Status = "expired";
                await _context.SaveChangesAsync();
            }

            var features = !string.IsNullOrEmpty(membership.MembershipType?.Features)
                ? JsonSerializer.Deserialize<string[]>(membership.MembershipType.Features) ?? Array.Empty<string>()
                : Array.Empty<string>();

            return Ok(ApiResponse.Success(new
            {
                HasMembership = !isExpired,
                MembershipType = membership.MembershipType?.Name,
                MembershipCode = membership.MembershipType?.Code,
                StartDate = membership.StartDate,
                EndDate = membership.EndDate,
                IsExpired = isExpired,
                AutoRenew = membership.AutoRenew,
                Features = features
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取会员信息失败");
            return StatusCode(500, ApiResponse.Error($"获取会员信息失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 检查内容访问权限
    /// </summary>
    [HttpPost("check-access")]
    public async Task<ActionResult<ApiResponse<object>>> CheckContentAccess([FromBody] ContentAccessRequest request)
    {
        try
        {
            var paidContent = await _context.PaidContents
                .FirstOrDefaultAsync(c => c.ContentType == request.ContentType && c.ContentId == request.ContentId);

            if (paidContent == null)
            {
                // 内容不存在或免费
                return Ok(ApiResponse.Success(new { HasAccess = true }));
            }

            // 检查是否已购买
            var hasPurchase = await _context.ContentPurchases
                .AnyAsync(p => 
                    p.UserId == request.UserId && 
                    p.ContentType == request.ContentType && 
                    p.ContentId == request.ContentId && 
                    p.PaymentStatus == "paid");

            if (hasPurchase)
            {
                return Ok(ApiResponse.Success(new { HasAccess = true }));
            }

            // 检查会员权限
            if (paidContent.IsMemberOnly || !string.IsNullOrEmpty(paidContent.RequiredMembership))
            {
                var membership = await _context.UserMemberships
                    .Include(m => m.MembershipType)
                    .Where(m => 
                        m.UserId == request.UserId && 
                        m.Status == "active" &&
                        (m.EndDate == null || m.EndDate > DateTime.Now))
                    .FirstOrDefaultAsync();

                if (membership != null)
                {
                    if (string.IsNullOrEmpty(paidContent.RequiredMembership) || 
                        membership.MembershipType?.Code == paidContent.RequiredMembership)
                    {
                        return Ok(ApiResponse.Success(new { HasAccess = true }));
                    }
                }
            }

            // 需要付费或会员
            return Ok(ApiResponse.Success(new
            {
                HasAccess = false,
                RequiresPayment = paidContent.Price > 0,
                RequiresMembership = paidContent.IsMemberOnly,
                Price = paidContent.Price,
                RequiredMembership = paidContent.RequiredMembership,
                PreviewContent = paidContent.PreviewContent
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查访问权限失败");
            return StatusCode(500, ApiResponse.Error($"检查访问权限失败: {ex.Message}", 500));
        }
    }
}

// 请求模型
public class MembershipPurchaseRequest
{
    public long MembershipTypeId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public bool? AutoRenew { get; set; }
}

public class ContentAccessRequest
{
    public string ContentType { get; set; } = string.Empty;
    public long ContentId { get; set; }
    public string UserId { get; set; } = string.Empty;
}

