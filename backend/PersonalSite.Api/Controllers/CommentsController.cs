using Microsoft.AspNetCore.Mvc;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 游客端评论：只读 approved，提交默认 pending
/// </summary>
[ApiController]
[Route("api/comments")]
public class CommentsController : ControllerBase
{
    private readonly IContentInteractionService _service;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CommentsController(IContentInteractionService service, IHttpContextAccessor httpContextAccessor)
    {
        _service = service;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>公开评论列表（仅 approved，不含 email）</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<PublicCommentDto>>>> List(
        [FromQuery] string targetType,
        [FromQuery] string targetId)
    {
        try
        {
            List<PublicCommentDto> list = await _service.ListApprovedCommentsAsync(targetType, targetId);
            return Ok(ApiResponse<List<PublicCommentDto>>.Success(list));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<List<PublicCommentDto>>.Error(ex.Message, 400));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<PublicCommentDto>>.Error($"获取失败: {ex.Message}", 500));
        }
    }

    /// <summary>提交回应（进入待审核）</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] CreateCommentRequestDto dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest(ApiResponse.Error("请求体不能为空", 400));
            }

            string? ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            await _service.CreateCommentAsync(dto, ip);
            return Ok(ApiResponse.Success(null, "回应已提交，审核通过后会展示。"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.Error(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(429, ApiResponse.Error(ex.Message, 429));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"提交失败: {ex.Message}", 500));
        }
    }
}
