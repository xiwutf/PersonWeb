using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 公开阅读摘录（仅 published + public）
/// </summary>
[ApiController]
[Route("api/reading")]
[AllowAnonymous]
public class PublicReadingController : ControllerBase
{
    private readonly IReadingService _readingService;
    private readonly ILogger<PublicReadingController> _logger;

    public PublicReadingController(
        IReadingService readingService,
        ILogger<PublicReadingController> logger)
    {
        _readingService = readingService;
        _logger = logger;
    }

    /// <summary>公开列表</summary>
    [HttpGet]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
    public async Task<ActionResult<ApiResponse<object>>> List(
        CancellationToken cancellationToken = default)
    {
        try
        {
            List<ReadingEntryDto> items = await _readingService.ListPublicAsync(cancellationToken);
            return Ok(ApiResponse.Success(new { Items = items }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list public reading entries");
            return StatusCode(500, ApiResponse.Error("Failed to load reading entries", 500));
        }
    }
}
