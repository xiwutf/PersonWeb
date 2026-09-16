using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 阅读 Inbox 管理（管理员 JWT）
/// </summary>
[ApiController]
[Route("api/admin/reading")]
[Authorize]
public class AdminReadingController : ControllerBase
{
    private readonly IReadingService _readingService;
    private readonly ILogger<AdminReadingController> _logger;

    public AdminReadingController(
        IReadingService readingService,
        ILogger<AdminReadingController> logger)
    {
        _readingService = readingService;
        _logger = logger;
    }

    /// <summary>列表</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> List(
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            List<ReadingEntryDto> items = await _readingService.ListAsync(status, cancellationToken);
            return Ok(ApiResponse.Success(new { Items = items }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list reading entries");
            return StatusCode(500, ApiResponse.Error(
                "Failed to load reading inbox. Ensure reading_entries table exists.",
                500));
        }
    }

    /// <summary>发布 / 归档</summary>
    [HttpPatch("{id:long}")]
    public async Task<ActionResult<ApiResponse<object>>> Patch(
        long id,
        [FromBody] ReadingActionRequest? body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ReadingEntryDto? item = await _readingService.ApplyActionAsync(
                id,
                body?.Action ?? string.Empty,
                cancellationToken);

            if (item == null)
            {
                return Ok(ApiResponse.Error("Reading entry not found", 404));
            }

            return Ok(ApiResponse.Success(new { Item = item }));
        }
        catch (ArgumentException ex)
        {
            return Ok(ApiResponse.Error(ex.Message, 400));
        }
    }

    /// <summary>删除</summary>
    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        long id,
        CancellationToken cancellationToken = default)
    {
        bool deleted = await _readingService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return Ok(ApiResponse.Error("Reading entry not found", 404));
        }

        return Ok(ApiResponse.Success(new { Ok = true }));
    }
}
