using System.Security.Claims;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PersonalSite.Api.Models;
using PersonalSite.Api.Services.Reading;
using PersonalSite.Api.Models.Dto;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/library")]
[EnableCors("ReadingOrigins")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class LibraryController(
    ReadingLibraryService library,
    ReadingCompanionAiService companionAi,
    ILogger<LibraryController> logger) : ControllerBase
{
    private string OwnerKey => (string)HttpContext.Items[ReadingSessionService.OwnerItem]!;

    [HttpPost("books")]
    [RequestSizeLimit(ReadingLibraryOptions.MaxUploadBytes + 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = ReadingLibraryOptions.MaxUploadBytes + 1024 * 1024)]
    public Task<IActionResult> Upload([FromForm] IFormFile file, CancellationToken ct) => Execute(async () =>
    {
        var book = await library.ImportAsync(OwnerKey, file, ct);
        return Created($"/api/library/books/{book.Id}", ApiResponse.Success(book));
    });

    [HttpGet("books")]
    public Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 24,
        [FromQuery] string? q = null, CancellationToken ct = default) => Execute(async () =>
            Ok(ApiResponse.Success(await library.ListAsync(OwnerKey, page, pageSize, q, ct))));

    [HttpGet("books/{id:long}")]
    public Task<IActionResult> Get(long id, CancellationToken ct) => Execute(async () =>
        Ok(ApiResponse.Success(await library.GetAsync(OwnerKey, id, ct))));

    [HttpGet("books/{id:long}/progress")]
    public Task<IActionResult> Progress(long id, CancellationToken ct) => Execute(async () =>
        Ok(ApiResponse.Success(await library.ProgressAsync(OwnerKey, id, ct))));

    [HttpPut("books/{id:long}/progress")]
    public Task<IActionResult> SaveProgress(long id, [FromBody] SaveReadingProgressRequest input, CancellationToken ct) => Execute(async () =>
        Ok(ApiResponse.Success(await library.SaveProgressAsync(OwnerKey, id, input, ct))));

    [HttpGet("annotations")]
    public Task<IActionResult> ListAnnotationFeed(CancellationToken ct) => Execute(async () =>
        Ok(ApiResponse.Success(await library.ListAnnotationFeedAsync(OwnerKey, ct))));

    [HttpGet("books/{id:long}/annotations")]
    public Task<IActionResult> ListAnnotations(long id, CancellationToken ct) => Execute(async () =>
        Ok(ApiResponse.Success(await library.ListAnnotationsAsync(OwnerKey, id, ct))));

    [HttpPost("books/{id:long}/annotations")]
    public Task<IActionResult> CreateAnnotation(long id, [FromBody] CreateReadingAnnotationRequest input, CancellationToken ct) => Execute(async () =>
    {
        ReadingAnnotationDto note = await library.CreateAnnotationAsync(OwnerKey, id, input, ct);
        return Created($"/api/library/books/{id}/annotations/{note.Id}", ApiResponse.Success(note));
    });

    [HttpPost("ai/explain")]
    public Task<IActionResult> Explain([FromBody] ReadingExplainRequest input, CancellationToken ct) => Execute(async () =>
        Ok(ApiResponse.Success(await companionAi.ExplainAsync(input, ct))));

    [HttpGet("continue")]
    public Task<IActionResult> Continue(CancellationToken ct) => Execute(async () =>
        Ok(ApiResponse.Success(await library.ContinueAsync(OwnerKey, ct))));

    [HttpGet("books/{id:long}/content")]
    public Task<IActionResult> Content(long id, CancellationToken ct) => Execute(async () =>
    {
        Response.Headers.XContentTypeOptions = "nosniff";
        Response.Headers.ContentSecurityPolicy = "default-src 'none'; sandbox";
        // Only a ZIP payload, never render archive XHTML on the API origin.
        return File(await library.OpenAsync(OwnerKey, id, ct), "application/epub+zip", $"book-{id}.epub", enableRangeProcessing: false);
    });

    [HttpGet("books/{id:long}/cover")]
    public Task<IActionResult> Cover(long id, CancellationToken ct) => Execute(async () =>
    {
        var cover = await library.CoverAsync(OwnerKey, id, ct);
        Response.Headers.XContentTypeOptions = "nosniff";
        Response.Headers.ContentSecurityPolicy = "default-src 'none'; sandbox";
        return File(cover.Bytes, cover.ContentType);
    });

    private async Task<IActionResult> Execute(Func<Task<IActionResult>> action)
    {
        try { return await action(); }
        catch (ReadingLibraryException ex)
        {
            return StatusCode(ex.StatusCode, new { code = ex.StatusCode, message = ex.Message, error = ex.Code });
        }
        catch (OperationCanceledException) when (HttpContext.RequestAborted.IsCancellationRequested) { throw; }
        catch (Exception ex)
        {
            logger.LogError(ex, "Reading library request failed");
            return StatusCode(500, ApiResponse.Error("Reading library is unavailable. Check server logs and database setup.", 500));
        }
    }
}
