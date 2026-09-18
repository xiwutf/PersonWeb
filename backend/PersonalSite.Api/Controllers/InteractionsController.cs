using Microsoft.AspNetCore.Mvc;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 内容互动：点赞与汇总（通用 targetType + targetId）
/// </summary>
[ApiController]
[Route("api/interactions")]
public class InteractionsController : ControllerBase
{
    private readonly IContentInteractionService _service;

    public InteractionsController(IContentInteractionService service)
    {
        _service = service;
    }

    /// <summary>获取点赞数 / 是否已赞 / 已通过评论数</summary>
    [HttpGet("{targetType}/{targetId}")]
    public async Task<ActionResult<ApiResponse<InteractionSummaryDto>>> GetSummary(
        string targetType,
        string targetId,
        [FromQuery] string? visitorId = null)
    {
        try
        {
            InteractionSummaryDto data = await _service.GetSummaryAsync(targetType, targetId, visitorId);
            return Ok(ApiResponse<InteractionSummaryDto>.Success(data));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<InteractionSummaryDto>.Error(ex.Message, 400));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<InteractionSummaryDto>.Error($"获取失败: {ex.Message}", 500));
        }
    }

    /// <summary>点赞</summary>
    [HttpPost("{targetType}/{targetId}/like")]
    public async Task<ActionResult<ApiResponse<object>>> Like(
        string targetType,
        string targetId,
        [FromBody] LikeRequestDto dto)
    {
        try
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.VisitorId))
            {
                return BadRequest(ApiResponse.Error("visitorId 不能为空", 400));
            }

            (bool created, int likeCount) = await _service.LikeAsync(targetType, targetId, dto.VisitorId);
            return Ok(ApiResponse.Success(new { liked = true, created, likeCount }, created ? "已点赞" : "已点过赞"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.Error(ex.Message, 400));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"点赞失败: {ex.Message}", 500));
        }
    }

    /// <summary>取消点赞（visitorId 走 query，避免 DELETE body 兼容问题）</summary>
    [HttpDelete("{targetType}/{targetId}/like")]
    public async Task<ActionResult<ApiResponse<object>>> Unlike(
        string targetType,
        string targetId,
        [FromQuery] string visitorId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(visitorId))
            {
                return BadRequest(ApiResponse.Error("visitorId 不能为空", 400));
            }

            (bool removed, int likeCount) = await _service.UnlikeAsync(targetType, targetId, visitorId);
            return Ok(ApiResponse.Success(new { liked = false, removed, likeCount }, removed ? "已取消点赞" : "未点过赞"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.Error(ex.Message, 400));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"取消失败: {ex.Message}", 500));
        }
    }
}
