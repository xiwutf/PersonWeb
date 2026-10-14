using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PersonalSite.Api.Services.Reading;

// Applies globally to every current and future MVC endpoint under /api/library.
public sealed class ReadingOwnerFilter(ReadingSessionService sessions, ILogger<ReadingOwnerFilter> logger) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var http = context.HttpContext;
        if (!http.Request.Path.StartsWithSegments("/api/library")) return;
        http.Response.Headers.CacheControl = "private, no-store";
        var path = http.Request.Path.Value?.TrimEnd('/');
        if ((path == "/api/library/auth/login" && HttpMethods.IsPost(http.Request.Method))
            || (path == "/api/library/auth/session" && HttpMethods.IsGet(http.Request.Method))) return;
        try
        {
            var session = await sessions.ValidateAsync(http, http.RequestAborted);
            if (session == null) { context.Result = new UnauthorizedResult(); return; }
            if (!(HttpMethods.IsGet(http.Request.Method) || HttpMethods.IsHead(http.Request.Method) || HttpMethods.IsOptions(http.Request.Method)))
                sessions.RequireCsrf(http);
            http.Items[ReadingSessionService.OwnerItem] = session.OwnerKey;
            http.Items[ReadingSessionService.SessionItem] = session;
            if (http.Request.ContentLength > ReadingLibraryOptions.MaxUploadBytes + 1024 * 1024)
                context.Result = new ObjectResult(new { error = "upload_too_large" }) { StatusCode = 413 };
        }
        catch (ReadingLibraryException ex) { context.Result = new ObjectResult(new { error = ex.Code, message = ex.Message }) { StatusCode = ex.StatusCode }; }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Reading session verification unavailable");
            context.Result = new ObjectResult(new { error = "reading_unavailable" }) { StatusCode = 503 };
        }
    }
}
