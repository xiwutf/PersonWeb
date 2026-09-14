using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Services.Cache;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisitorInteractionController : ControllerBase
{
    private const string ApprovedDanmakuCacheKey = "visitor:danmaku:approved:v1";
    private static readonly TimeSpan ApprovedDanmakuCacheTtl = TimeSpan.FromSeconds(45);

    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICacheService _cache;

    public VisitorInteractionController(
        AppDbContext context,
        IHttpContextAccessor httpContextAccessor,
        ICacheService cache)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
    }

    /// <summary>
    /// 发送访客留言/弹幕
    /// </summary>
    [HttpPost("message")]
    public async Task<ActionResult<ApiResponse<object>>> SendMessage([FromBody] VisitorMessageDto dto)
    {
        try
        {
            if (string.IsNullOrEmpty(dto.VisitorId) || string.IsNullOrEmpty(dto.Content))
            {
                return BadRequest(ApiResponse.Error("访客ID和内容不能为空", 400));
            }

            string? ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

            VisitorMessage message = new VisitorMessage
            {
                VisitorId = dto.VisitorId,
                VisitorName = string.IsNullOrWhiteSpace(dto.VisitorName) ? null : dto.VisitorName.Trim(),
                MessageType = dto.MessageType ?? "message",
                Content = dto.Content,
                Emoji = dto.Emoji,
                Color = dto.Color,
                Status = "pending", // 需要审核
                Ip = ip,
                Location = dto.Location,
                CreatedAt = DateTime.Now
            };

            _context.VisitorMessages.Add(message);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new { id = message.Id }, "留言已提交，等待审核"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"发送失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取已审核的弹幕（用于显示）
    /// </summary>
    [HttpGet("messages/approved")]
    public async Task<ActionResult<ApiResponse<List<VisitorDanmakuItemDto>>>> GetApprovedMessages([FromQuery] int limit = 36)
    {
        try
        {
            // 首页弹幕最多展示 36 条，超出部分无意义且拖慢载荷
            int safeLimit = Math.Clamp(limit, 1, 36);

            List<VisitorDanmakuItemDto>? cached = await _cache.GetAsync<List<VisitorDanmakuItemDto>>(ApprovedDanmakuCacheKey);
            if (cached != null)
            {
                List<VisitorDanmakuItemDto> cachedSlice = cached.Take(safeLimit).ToList();
                return Ok(ApiResponse<List<VisitorDanmakuItemDto>>.Success(cachedSlice));
            }

            // 只投影弹幕展示字段，避免整行实体与敏感字段出站
            List<VisitorDanmakuItemDto> messages = await _context.VisitorMessages
                .AsNoTracking()
                .Where(m => m.Status == "approved")
                .OrderByDescending(m => m.ApprovedAt ?? m.CreatedAt)
                .Take(36)
                .Select(m => new VisitorDanmakuItemDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    Emoji = m.Emoji,
                    Color = m.Color,
                    MessageType = m.MessageType,
                })
                .ToListAsync();

            // 兼容旧「时间胶囊」已展示内容，统一进入弹幕流
            int remain = Math.Max(0, 36 - messages.Count);
            if (remain > 0)
            {
                List<VisitorDanmakuItemDto> legacyCapsules = await _context.TimeCapsules
                    .AsNoTracking()
                    .Where(t => t.Status == 1)
                    .OrderByDescending(t => t.CreatedAt)
                    .Take(remain)
                    .Select(t => new VisitorDanmakuItemDto
                    {
                        Id = 1_000_000_000L + t.Id,
                        Content = t.Content,
                        Emoji = null,
                        Color = null,
                        MessageType = "message",
                    })
                    .ToListAsync();

                messages.AddRange(legacyCapsules);
            }

            await _cache.SetAsync(ApprovedDanmakuCacheKey, messages, ApprovedDanmakuCacheTtl);

            List<VisitorDanmakuItemDto> result = messages.Take(safeLimit).ToList();
            return Ok(ApiResponse<List<VisitorDanmakuItemDto>>.Success(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<VisitorDanmakuItemDto>>.Error($"获取失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 审核留言（管理员）
    /// </summary>
    [HttpPost("message/{id}/approve")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> ApproveMessage(long id)
    {
        try
        {
            VisitorMessage? message = await _context.VisitorMessages.FindAsync(id);
            if (message == null)
            {
                return NotFound(ApiResponse.Error("留言不存在", 404));
            }

            message.Status = "approved";
            message.ApprovedAt = DateTime.Now;
            message.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            await InvalidateApprovedDanmakuCacheAsync();

            return Ok(ApiResponse.Success(null, "审核通过"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"审核失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 拒绝留言（管理员）
    /// </summary>
    [HttpPost("message/{id}/reject")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> RejectMessage(long id)
    {
        try
        {
            VisitorMessage? message = await _context.VisitorMessages.FindAsync(id);
            if (message == null)
            {
                return NotFound(ApiResponse.Error("留言不存在", 404));
            }

            message.Status = "rejected";
            message.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            await InvalidateApprovedDanmakuCacheAsync();

            return Ok(ApiResponse.Success(null, "已拒绝"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"操作失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 留下足迹
    /// </summary>
    [HttpPost("footprint")]
    public async Task<ActionResult<ApiResponse<object>>> LeaveFootprint([FromBody] VisitorFootprintDto dto)
    {
        try
        {
            if (string.IsNullOrEmpty(dto.VisitorId) || string.IsNullOrEmpty(dto.Emoji))
            {
                return BadRequest(ApiResponse.Error("访客ID和表情不能为空", 400));
            }

            string? ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

            VisitorFootprint footprint = new VisitorFootprint
            {
                VisitorId = dto.VisitorId,
                Emoji = dto.Emoji,
                IconType = dto.IconType ?? "emoji",
                Message = dto.Message,
                XPosition = dto.XPosition,
                YPosition = dto.YPosition,
                Ip = ip,
                Location = dto.Location,
                CreatedAt = DateTime.Now
            };

            _context.VisitorFootprints.Add(footprint);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new { id = footprint.Id }, "足迹已留下"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"留下足迹失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取所有足迹
    /// </summary>
    [HttpGet("footprints")]
    public async Task<ActionResult<ApiResponse<List<VisitorFootprint>>>> GetFootprints()
    {
        try
        {
            List<VisitorFootprint> footprints = await _context.VisitorFootprints
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return Ok(ApiResponse<List<VisitorFootprint>>.Success(footprints));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<VisitorFootprint>>.Error($"获取失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 记录访客气泡（新访客进入）
    /// </summary>
    [HttpPost("bubble")]
    public async Task<ActionResult<ApiResponse<object>>> CreateBubble([FromBody] VisitorBubbleDto dto)
    {
        try
        {
            if (string.IsNullOrEmpty(dto.VisitorId))
            {
                return BadRequest(ApiResponse.Error("访客ID不能为空", 400));
            }

            VisitorBubble bubble = new VisitorBubble
            {
                VisitorId = dto.VisitorId,
                AvatarUrl = dto.AvatarUrl,
                Location = dto.Location,
                DisplayText = dto.DisplayText,
                DisplayedAt = DateTime.Now
            };

            _context.VisitorBubbles.Add(bubble);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new { id = bubble.Id }, "气泡已创建"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"创建失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取待审核的留言（管理员）
    /// </summary>
    [HttpGet("messages/pending")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<VisitorMessage>>>> GetPendingMessages()
    {
        try
        {
            List<VisitorMessage> messages = await _context.VisitorMessages
                .Where(m => m.Status == "pending")
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return Ok(ApiResponse<List<VisitorMessage>>.Success(messages));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<VisitorMessage>>.Error($"获取失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取所有留言（管理员，用于管理页面）
    /// </summary>
    [HttpGet("messages/all")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<VisitorMessage>>>> GetAllMessages([FromQuery] string? status = null, [FromQuery] string? messageType = null)
    {
        try
        {
            IQueryable<VisitorMessage> query = _context.VisitorMessages.AsQueryable();

            // 根据状态筛选
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(m => m.Status == status);
            }

            // 根据类型筛选
            if (!string.IsNullOrEmpty(messageType))
            {
                query = query.Where(m => m.MessageType == messageType);
            }

            List<VisitorMessage> messages = await query
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return Ok(ApiResponse<List<VisitorMessage>>.Success(messages));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<VisitorMessage>>.Error($"获取失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 审核状态变更后清除弹幕缓存
    /// </summary>
    private Task InvalidateApprovedDanmakuCacheAsync()
    {
        return _cache.RemoveAsync(ApprovedDanmakuCacheKey);
    }
}

/// <summary>
/// 入口页弹幕精简载荷（不含访客敏感字段）
/// </summary>
public class VisitorDanmakuItemDto
{
    public long Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Emoji { get; set; }
    public string? Color { get; set; }
    public string? MessageType { get; set; }
}

// DTOs
public class VisitorMessageDto
{
    public string VisitorId { get; set; } = string.Empty;
    public string? VisitorName { get; set; }
    public string? MessageType { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Emoji { get; set; }
    public string? Color { get; set; }
    public string? Location { get; set; }
}

public class VisitorFootprintDto
{
    public string VisitorId { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public string? IconType { get; set; }
    public string? Message { get; set; }
    public decimal? XPosition { get; set; }
    public decimal? YPosition { get; set; }
    public string? Location { get; set; }
}

public class VisitorBubbleDto
{
    public string VisitorId { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Location { get; set; }
    public string? DisplayText { get; set; }
}
