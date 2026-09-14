using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;
using PersonalSite.Api.Utils;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// MindTrace 外部发布接入（Bearer IngestToken，非管理员 JWT）
/// </summary>
[ApiController]
[Route("api/integrations/mindtrace")]
[AllowAnonymous]
public class MindtraceIntegrationController : ControllerBase
{
    private readonly IReadingService _readingService;
    private readonly MindtraceIngestOptions _options;
    private readonly ILogger<MindtraceIntegrationController> _logger;

    public MindtraceIntegrationController(
        IReadingService readingService,
        IOptions<MindtraceIngestOptions> options,
        ILogger<MindtraceIntegrationController> logger)
    {
        _readingService = readingService;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>连通性探测</summary>
    [HttpGet]
    public IActionResult Probe()
    {
        if (!TryAuthorizeIngest(out IActionResult? error))
        {
            return error!;
        }

        return Ok(new { ok = true, service = "mindtrace-ingest" });
    }

    /// <summary>接收 MindTrace Thought</summary>
    [HttpPost]
    public async Task<IActionResult> Ingest(
        [FromBody] MindtraceIngestRequest? body,
        CancellationToken cancellationToken)
    {
        if (!TryAuthorizeIngest(out IActionResult? error))
        {
            return error!;
        }

        try
        {
            (bool created, ReadingEntryDto entry) = await _readingService.IngestMindtraceAsync(
                body ?? new MindtraceIngestRequest(),
                cancellationToken);

            return Ok(new
            {
                ok = true,
                created,
                id = entry.Id,
                status = entry.Status,
                visibility = entry.Visibility,
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { ok = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MindTrace ingest failed");
            return StatusCode(500, new { ok = false, message = "ingest failed" });
        }
    }

    private bool TryAuthorizeIngest(out IActionResult? error)
    {
        error = null;
        (int StatusCode, string Message)? failure = MindtraceIngestAuth.Validate(
            Request.Headers.Authorization.ToString(),
            _options.IngestToken);

        if (failure == null)
        {
            return true;
        }

        error = StatusCode(failure.Value.StatusCode, new
        {
            ok = false,
            message = failure.Value.Message,
        });
        return false;
    }
}
