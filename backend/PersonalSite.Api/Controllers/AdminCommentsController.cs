using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 管理员评论审核
/// </summary>
[ApiController]
[Authorize]
[Route("api/admin/comments")]
public class AdminCommentsController : ControllerBase
{
    private readonly IContentInteractionService _service;

    public AdminCommentsController(IContentInteractionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<AdminCommentListDto>>> List(
        [FromQuery] string? status = "pending",
        [FromQuery] string? targetType = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            AdminCommentListDto data = await _service.ListAdminCommentsAsync(status, targetType, page, pageSize);
            return Ok(ApiResponse<AdminCommentListDto>.Success(data));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<AdminCommentListDto>.Error($"获取失败: {ex.Message}", 500));
        }
    }

    [HttpPatch("{id:long}/approve")]
    public async Task<ActionResult<ApiResponse>> Approve(long id)
    {
        try
        {
            ContentComment? comment = await _service.ApproveAsync(id);
            if (comment == null)
            {
                return NotFound(ApiResponse.Error("评论不存在", 404));
            }
            return Ok(ApiResponse.Success(null, "已通过"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"操作失败: {ex.Message}", 500));
        }
    }

    [HttpPatch("{id:long}/reject")]
    public async Task<ActionResult<ApiResponse>> Reject(long id)
    {
        try
        {
            ContentComment? comment = await _service.RejectAsync(id);
            if (comment == null)
            {
                return NotFound(ApiResponse.Error("评论不存在", 404));
            }
            return Ok(ApiResponse.Success(null, "已拒绝"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"操作失败: {ex.Message}", 500));
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse>> Delete(long id)
    {
        try
        {
            bool ok = await _service.DeleteAsync(id);
            if (!ok)
            {
                return NotFound(ApiResponse.Error("评论不存在", 404));
            }
            return Ok(ApiResponse.Success(null, "已删除"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"删除失败: {ex.Message}", 500));
        }
    }
}
