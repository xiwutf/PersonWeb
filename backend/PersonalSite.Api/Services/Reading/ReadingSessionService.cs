using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Services.Reading;

public sealed class ReadingSessionService(AppDbContext db, IOptions<ReadingLibraryOptions> options, TimeProvider clock)
{
    public const string CookieName = "__Secure-personweb_reading";
    public const string OwnerItem = "ReadingOwnerKey";
    public const string SessionItem = "ReadingSession";
    public const string CsrfHeader = "X-Reading-CSRF";
    private ReadingLibraryOptions Settings => options.Value;
    private static readonly PasswordHasher<object> Hasher = new();
    private static readonly object HashUser = new();
    public static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
    public static string Csrf(string token) => Hash("personweb-reading-csrf:" + token);

    public void RequireConfigured()
    {
        var validHash = false;
        try
        {
            var bytes = Convert.FromBase64String(Settings.AccessPasswordHash);
            validHash = bytes.Length == 61 && bytes[0] == 1
                && BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(1)) == 2
                && BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(5)) is >= 100000 and <= 1000000
                && BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(9)) == 16;
        }
        catch (FormatException) { }
        if (!Guid.TryParseExact(Settings.OwnerKey, "D", out var owner) || owner == Guid.Empty
            || Settings.OwnerKey != owner.ToString("D") || !validHash
            || Settings.SessionDays is < 1 or > 30 || Settings.AbsoluteSessionDays < Settings.SessionDays
            || Settings.AbsoluteSessionDays > 90 || Settings.AllowedOrigins.Length == 0
            || Settings.AllowedOrigins.Any(origin => !Uri.TryCreate(origin, UriKind.Absolute, out var uri)
                || uri.GetLeftPart(UriPartial.Authority) != origin
                || (uri.Scheme != "https" && !(uri.Scheme == "http" && uri.IsLoopback))))
            throw new ReadingLibraryException(503, "reading_not_configured", "Reading access is not securely configured.");
    }

    public void RequireOrigin(HttpContext context)
    {
        RequireConfigured();
        if (!Settings.AllowedOrigins.Contains(context.Request.Headers.Origin.ToString(), StringComparer.Ordinal))
            throw new ReadingLibraryException(403, "origin_rejected", "Request origin is not allowed.");
    }

    public void RequireCsrf(HttpContext context)
    {
        RequireOrigin(context);
        var token = context.Request.Cookies[CookieName] ?? "";
        var provided = context.Request.Headers[CsrfHeader].ToString();
        if (provided.Length != 64 || !CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(provided), Encoding.UTF8.GetBytes(Csrf(token))))
            throw new ReadingLibraryException(403, "csrf_rejected", "CSRF verification failed.");
    }

    public async Task<ReadingSession?> ValidateAsync(HttpContext context, CancellationToken ct)
    {
        RequireConfigured();
        var token = context.Request.Cookies[CookieName];
        if (token is null || token.Length != 64 || !token.All(Uri.IsHexDigit)) return null;
        var hash = Hash(token);
        var session = await db.ReadingSessions.AsNoTracking().SingleOrDefaultAsync(s => s.TokenHash == hash, ct);
        var now = clock.GetUtcNow().UtcDateTime;
        if (session == null || session.RevokedAt != null || session.ExpiresAt <= now || session.AbsoluteExpiresAt <= now
            || session.OwnerKey != Settings.OwnerKey || session.CredentialVersion != Hash(Settings.AccessPasswordHash)) return null;
        return session;
    }

    public async Task<object> LoginAsync(HttpContext context, string password, CancellationToken ct)
    {
        RequireOrigin(context);
        if (password.Length is < 1 or > 1024 || Hasher.VerifyHashedPassword(HashUser, Settings.AccessPasswordHash, password)
            == PasswordVerificationResult.Failed)
            throw new ReadingLibraryException(401, "invalid_password", "Incorrect reading password.");
        var now = clock.GetUtcNow().UtcDateTime;
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
        var session = new ReadingSession { TokenHash = Hash(token), OwnerKey = Settings.OwnerKey,
            CredentialVersion = Hash(Settings.AccessPasswordHash), CreatedAt = now,
            ExpiresAt = now.AddDays(Settings.SessionDays), AbsoluteExpiresAt = now.AddDays(Settings.AbsoluteSessionDays) };
        var previous = await ValidateAsync(context, ct);
        if (previous != null) { db.Attach(previous); previous.RevokedAt = now; }
        db.ReadingSessions.Add(session);
        await db.SaveChangesAsync(ct);
        SetCookie(context, token, session.ExpiresAt);
        return View(context, session, token);
    }

    public object View(HttpContext context, ReadingSession session, string? token = null) => new {
        authenticated = true, expiresAt = DateTime.SpecifyKind(session.ExpiresAt, DateTimeKind.Utc),
        csrfToken = Csrf(token ?? context.Request.Cookies[CookieName]!)
    };

    public async Task<object> RenewAsync(HttpContext context, ReadingSession session, CancellationToken ct)
    {
        RequireCsrf(context);
        var now = clock.GetUtcNow().UtcDateTime;
        var expires = new[] { now.AddDays(Settings.SessionDays), session.AbsoluteExpiresAt }.Min();
        // A conditional update prevents renewal from undoing a concurrent logout.
        if (db.Database.IsRelational())
        {
            var count = await db.ReadingSessions.Where(s => s.TokenHash == session.TokenHash && s.RevokedAt == null
                && s.ExpiresAt > now && s.AbsoluteExpiresAt > now).ExecuteUpdateAsync(
                updates => updates.SetProperty(s => s.ExpiresAt, expires), ct);
            if (count != 1) throw new ReadingLibraryException(401, "session_expired", "Reading session has expired.");
        }
        else { db.Attach(session); session.ExpiresAt = expires; await db.SaveChangesAsync(ct); }
        session.ExpiresAt = expires;
        SetCookie(context, context.Request.Cookies[CookieName]!, expires);
        return View(context, session);
    }

    public async Task LogoutAsync(HttpContext context, ReadingSession session, CancellationToken ct)
    {
        RequireCsrf(context);
        db.Attach(session);
        session.RevokedAt = clock.GetUtcNow().UtcDateTime;
        await db.SaveChangesAsync(ct);
        context.Response.Cookies.Delete(CookieName, CookieOptions());
    }

    private static CookieOptions CookieOptions() => new() { HttpOnly = true, Secure = true,
        SameSite = SameSiteMode.Strict, Path = "/api/library", IsEssential = true };
    private static void SetCookie(HttpContext context, string token, DateTime expires)
    {
        var cookie = CookieOptions(); cookie.Expires = new DateTimeOffset(expires, TimeSpan.Zero);
        context.Response.Cookies.Append(CookieName, token, cookie);
    }
}
