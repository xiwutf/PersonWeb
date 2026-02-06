using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 思维记录 API：CRUD + 请求 AI 批注
/// </summary>
[ApiController]
[Route("api/thoughts")]
public class ThoughtsController : ControllerBase
{
    private readonly IThoughtService _thoughtService;

    public ThoughtsController(IThoughtService thoughtService)
    {
        _thoughtService = thoughtService;
    }

    /// <summary>分页列表，可选 keyword</summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetPage(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var (total, list) = await _thoughtService.GetPageAsync(page, pageSize, keyword, cancellationToken);
        return Ok(ApiResponse.Success(new { Total = total, List = list }));
    }

    /// <summary>按 ID 获取详情</summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ThoughtDto>>> GetById(long id, CancellationToken cancellationToken = default)
    {
        var dto = await _thoughtService.GetByIdAsync(id, cancellationToken);
        if (dto == null)
        {
            return Ok(ApiResponse<ThoughtDto>.Error("思维记录不存在", 404));
        }
        return Ok(ApiResponse<ThoughtDto>.Success(dto));
    }

    /// <summary>创建一条思维记录</summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ThoughtDto>>> Create([FromBody] ThoughtCreateDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Content))
        {
            return Ok(ApiResponse<ThoughtDto>.Error("内容不能为空", 400));
        }

        try
        {
            var result = await _thoughtService.CreateAsync(dto.Content.Trim(), cancellationToken);
            return Ok(ApiResponse<ThoughtDto>.Success(result));
        }
        catch (ArgumentException ex)
        {
            return Ok(ApiResponse<ThoughtDto>.Error(ex.Message, 400));
        }
    }

    /// <summary>更新原文</summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ThoughtDto>>> Update(long id, [FromBody] ThoughtUpdateDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Content))
        {
            return Ok(ApiResponse<ThoughtDto>.Error("内容不能为空", 400));
        }

        try
        {
            var result = await _thoughtService.UpdateAsync(id, dto.Content.Trim(), cancellationToken);
            if (result == null)
            {
                return Ok(ApiResponse<ThoughtDto>.Error("思维记录不存在", 404));
            }
            return Ok(ApiResponse<ThoughtDto>.Success(result));
        }
        catch (ArgumentException ex)
        {
            return Ok(ApiResponse<ThoughtDto>.Error(ex.Message, 400));
        }
    }

    /// <summary>请求 AI 批注并写回</summary>
    [HttpPost("{id}/ai-comment")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ThoughtDto>>> GenerateAiComment(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _thoughtService.GenerateAiCommentAsync(id, cancellationToken);
            if (result == null)
            {
                return Ok(ApiResponse<ThoughtDto>.Error("思维记录不存在", 404));
            }
            return Ok(ApiResponse<ThoughtDto>.Success(result));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse<ThoughtDto>.Error(ex.Message, 500));
        }
    }
}
