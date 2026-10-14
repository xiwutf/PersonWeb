using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;
using PersonalSite.Api.Services.Reading;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 公开阅读摘录（仅 published + public）与站主读书笔记时间线
/// </summary>
[ApiController]
[Route("api/reading")]
[AllowAnonymous]
public class PublicReadingController : ControllerBase
{
    private readonly IReadingService _readingService;
    private readonly ReadingLibraryService _library;
    private readonly ReadingLibraryOptions _libraryOptions;
    private readonly ILogger<PublicReadingController> _logger;

    public PublicReadingController(
        IReadingService readingService,
        ReadingLibraryService library,
        IOptions<ReadingLibraryOptions> libraryOptions,
        ILogger<PublicReadingController> logger)
    {
        _readingService = readingService;
        _library = library;
        _libraryOptions = libraryOptions.Value;
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

    /// <summary>Life「读书」公开笔记（站主 OwnerKey，不含 EPUB 正文）</summary>
    [HttpGet("book-notes")]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any)]
    public async Task<ActionResult<ApiResponse<object>>> BookNotes(CancellationToken cancellationToken = default)
    {
        try
        {
            string ownerKey = (_libraryOptions.OwnerKey ?? "").Trim();
            if (string.IsNullOrEmpty(ownerKey))
                return StatusCode(503, ApiResponse.Error("Reading notes are not configured", 503));
            IReadOnlyList<ReadingAnnotationFeedItem> items =
                await _library.ListAnnotationFeedAsync(ownerKey, cancellationToken);
            return Ok(ApiResponse.Success(items));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list public book notes");
            return StatusCode(500, ApiResponse.Error("Failed to load book notes", 500));
        }
    }
}
