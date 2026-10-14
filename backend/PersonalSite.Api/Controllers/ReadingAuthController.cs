using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PersonalSite.Api.Models;
using PersonalSite.Api.Services.Reading;

namespace PersonalSite.Api.Controllers;

[ApiController, Route("api/library/auth"), EnableCors("ReadingOrigins")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class ReadingAuthController(ReadingSessionService sessions, ILogger<ReadingAuthController> logger) : ControllerBase
{
    public sealed record PasswordRequest([Required, MaxLength(1024)] string Password);
    [HttpPost("login"), EnableRateLimiting("ReadingPassword"), RequestSizeLimit(4096)]
    public Task<IActionResult> Login(PasswordRequest request, CancellationToken ct) => Execute(async () =>
        Ok(await sessions.LoginAsync(HttpContext, request.Password, ct)));
    [HttpGet("session")]
    public Task<IActionResult> Session(CancellationToken ct) => Execute(async () => {
        var session = await sessions.ValidateAsync(HttpContext, ct);
        return session == null ? Unauthorized(new { authenticated = false }) : Ok(sessions.View(HttpContext, session));
    });
    [HttpPost("renew")]
    public Task<IActionResult> Renew(CancellationToken ct) => Execute(async () =>
        Ok(await sessions.RenewAsync(HttpContext, (ReadingSession)HttpContext.Items[ReadingSessionService.SessionItem]!, ct)));
    [HttpPost("logout")]
    public Task<IActionResult> Logout(CancellationToken ct) => Execute(async () => {
        await sessions.LogoutAsync(HttpContext, (ReadingSession)HttpContext.Items[ReadingSessionService.SessionItem]!, ct);
        return NoContent();
    });
    private async Task<IActionResult> Execute(Func<Task<IActionResult>> action)
    {
        Response.Headers.CacheControl = "private, no-store";
        try { return await action(); }
        catch (ReadingLibraryException ex) { return StatusCode(ex.StatusCode, new { error = ex.Code, message = ex.Message }); }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Reading authentication unavailable");
            return StatusCode(503, new { error = "reading_unavailable", message = "Reading access is unavailable." });
        }
    }
}
