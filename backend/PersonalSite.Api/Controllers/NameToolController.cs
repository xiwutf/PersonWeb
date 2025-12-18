using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;
using System.Text.Json;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 智能取名助手控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NameToolController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly INameToolAiService _aiService;
    private readonly ILogger<NameToolController> _logger;

    public NameToolController(
        AppDbContext context,
        INameToolAiService aiService,
        ILogger<NameToolController> logger)
    {
        _context = context;
        _aiService = aiService;
        _logger = logger;
    }

    /// <summary>
    /// 生成名字
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<ApiResponse<NameGenerateResponseDto>>> Generate(
        [FromBody] NameGenerateRequestDto request)
    {
        try
        {
            // 验证请求
            if (string.IsNullOrEmpty(request.Type))
            {
                return BadRequest(ApiResponse<NameGenerateResponseDto>.Error("取名类型不能为空"));
            }

            if (request.Style == null || request.Style.Count == 0)
            {
                return BadRequest(ApiResponse<NameGenerateResponseDto>.Error("至少选择一个风格"));
            }

            // 调用 AI 服务生成名字
            var response = await _aiService.GenerateAsync(request);

            return Ok(ApiResponse<NameGenerateResponseDto>.Success(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成名字失败");
            return StatusCode(500, ApiResponse<NameGenerateResponseDto>.Error(
                $"生成名字失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 再来一批（重新生成）
    /// </summary>
    [HttpPost("regenerate")]
    public async Task<ActionResult<ApiResponse<NameGenerateResponseDto>>> Regenerate(
        [FromBody] NameGenerateRequestDto request)
    {
        try
        {
            // 验证请求
            if (string.IsNullOrEmpty(request.Type))
            {
                return BadRequest(ApiResponse<NameGenerateResponseDto>.Error("取名类型不能为空"));
            }

            if (request.Style == null || request.Style.Count == 0)
            {
                return BadRequest(ApiResponse<NameGenerateResponseDto>.Error("至少选择一个风格"));
            }

            if (string.IsNullOrEmpty(request.TraceId))
            {
                return BadRequest(ApiResponse<NameGenerateResponseDto>.Error("traceId 不能为空"));
            }

            // 使用相同的 traceId 重新生成
            var response = await _aiService.GenerateAsync(request, request.TraceId);

            return Ok(ApiResponse<NameGenerateResponseDto>.Success(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重新生成名字失败");
            return StatusCode(500, ApiResponse<NameGenerateResponseDto>.Error(
                $"重新生成失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取收藏列表
    /// </summary>
    [HttpGet("favorites")]
    public async Task<ActionResult<ApiResponse<FavoriteListResponseDto>>> GetFavorites(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            // 当前使用匿名收藏，OwnerKey 为空或固定值
            // 后续可以改为从用户认证中获取
            var ownerKey = GetOwnerKey();

            var query = _context.NameFavorites.AsQueryable();

            // 如果指定了 ownerKey，则过滤
            if (!string.IsNullOrEmpty(ownerKey))
            {
                query = query.Where(f => f.OwnerKey == ownerKey);
            }
            else
            {
                // 匿名模式：返回所有收藏（或可以根据其他标识过滤）
                query = query.Where(f => f.OwnerKey == null || f.OwnerKey == "");
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new NameFavoriteDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Type = f.Type,
                    Style = f.Style ?? "",
                    Language = f.Language ?? "",
                    TotalScore = f.TotalScore,
                    Reason = f.Reason ?? "",
                    MetaJson = f.MetaJson,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                })
                .ToListAsync();

            var response = new FavoriteListResponseDto
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize
            };

            return Ok(ApiResponse<FavoriteListResponseDto>.Success(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取收藏列表失败");
            return StatusCode(500, ApiResponse<FavoriteListResponseDto>.Error(
                $"获取收藏列表失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 收藏名字
    /// </summary>
    [HttpPost("favorites")]
    public async Task<ActionResult<ApiResponse<NameFavoriteDto>>> AddFavorite(
        [FromBody] FavoriteCreateRequestDto request)
    {
        try
        {
            // 验证请求
            if (string.IsNullOrEmpty(request.Name))
            {
                return BadRequest(ApiResponse<NameFavoriteDto>.Error("名字不能为空"));
            }

            // 检查是否已收藏（同一 ownerKey 下）
            var ownerKey = GetOwnerKey();
            var existing = await _context.NameFavorites
                .FirstOrDefaultAsync(f =>
                    f.Name == request.Name &&
                    (string.IsNullOrEmpty(ownerKey) ? (f.OwnerKey == null || f.OwnerKey == "") : f.OwnerKey == ownerKey));

            if (existing != null)
            {
                // 已存在，返回现有记录
                var existingDto = new NameFavoriteDto
                {
                    Id = existing.Id,
                    Name = existing.Name,
                    Type = existing.Type,
                    Style = existing.Style ?? "",
                    Language = existing.Language ?? "",
                    TotalScore = existing.TotalScore,
                    Reason = existing.Reason ?? "",
                    MetaJson = existing.MetaJson,
                    CreatedAt = existing.CreatedAt,
                    UpdatedAt = existing.UpdatedAt
                };
                return Ok(ApiResponse<NameFavoriteDto>.Success(existingDto, "已收藏"));
            }

            // 构建 MetaJson
            var metaJson = JsonSerializer.Serialize(new
            {
                scores = request.Scores,
                requestMeta = request.RequestMeta
            });

            // 创建收藏记录
            var favorite = new NameFavorite
            {
                Name = request.Name,
                Type = request.Type,
                Style = string.Join(",", request.Style),
                Language = request.Language,
                TotalScore = request.TotalScore,
                Reason = request.Reason,
                MetaJson = metaJson,
                OwnerKey = ownerKey,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.NameFavorites.Add(favorite);
            await _context.SaveChangesAsync();

            var dto = new NameFavoriteDto
            {
                Id = favorite.Id,
                Name = favorite.Name,
                Type = favorite.Type,
                Style = favorite.Style ?? "",
                Language = favorite.Language ?? "",
                TotalScore = favorite.TotalScore,
                Reason = favorite.Reason ?? "",
                MetaJson = favorite.MetaJson,
                CreatedAt = favorite.CreatedAt,
                UpdatedAt = favorite.UpdatedAt
            };

            return Ok(ApiResponse<NameFavoriteDto>.Success(dto, "收藏成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "收藏名字失败");
            return StatusCode(500, ApiResponse<NameFavoriteDto>.Error(
                $"收藏失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 取消收藏
    /// </summary>
    [HttpDelete("favorites/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveFavorite(long id)
    {
        try
        {
            var ownerKey = GetOwnerKey();
            var favorite = await _context.NameFavorites
                .FirstOrDefaultAsync(f => f.Id == id &&
                    (string.IsNullOrEmpty(ownerKey) ? (f.OwnerKey == null || f.OwnerKey == "") : f.OwnerKey == ownerKey));

            if (favorite == null)
            {
                return NotFound(ApiResponse<object>.Error("收藏记录不存在"));
            }

            _context.NameFavorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Success(new { }, "取消收藏成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取消收藏失败");
            return StatusCode(500, ApiResponse<object>.Error(
                $"取消收藏失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取所有者标识（当前使用匿名，后续可改为从用户认证中获取）
    /// </summary>
    private string? GetOwnerKey()
    {
        // 当前实现：匿名收藏，返回 null
        // 后续可以改为：
        // - 从 JWT Token 中获取用户ID
        // - 从 Cookie 中获取访客ID
        // - 从请求头中获取标识
        
        // 示例：从请求头获取（如果前端传递了 visitor_id）
        if (Request.Headers.TryGetValue("X-Visitor-Id", out var visitorId))
        {
            return visitorId.ToString();
        }

        return null; // 匿名模式
    }
}

