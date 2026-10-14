using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using PersonalSite.Api.Controllers;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Services.Reading;
using Xunit;

namespace PersonalSite.Api.Tests.ReadingCenter;

// Real Kestrel HTTP, MVC, cookies, CORS/CSRF, physical files. No real MySQL writes.
public class ReadingLibraryHttpTests
{
    private const string Owner = "11111111-1111-4111-8111-111111111111";
    private const string Origin = "https://reading.example.test";
    private const string LegacyKey = "LegacyBlogTestKey_OnlyForTests_01234567890123456789";
    private static readonly string PasswordHash = new PasswordHasher<object>().HashPassword(new(), "reading-test-password");


    [Fact]
    public async Task PasswordLogin_UploadListGetPrivateDownload_AndPersistentCookie_Work()
    {
        await using var host = await Host.Create();
        var login = await host.Login();
        Assert.Contains("httponly", login.cookie.ToLowerInvariant());
        Assert.Contains("secure", login.cookie.ToLowerInvariant());
        Assert.Contains("samesite=strict", login.cookie.ToLowerInvariant());
        Assert.Contains("path=/api/library", login.cookie.ToLowerInvariant());
        Assert.Contains("expires=", login.cookie.ToLowerInvariant());
        Assert.DoesNotContain("domain=", login.cookie.ToLowerInvariant());
        var bytes = EpubTestFiles.Create();
        using var upload = await host.Upload(bytes);
        Assert.Equal(HttpStatusCode.Created, upload.StatusCode);
        var json = await upload.Content.ReadAsStringAsync();
        Assert.DoesNotContain("storageKey", json); Assert.DoesNotContain("ownerKey", json);
        var id = JsonDocument.Parse(json).RootElement.GetProperty("data").GetProperty("id").GetInt64();
        using var list = await host.Client.GetAsync("/api/library/books");
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        Assert.Equal(1, JsonDocument.Parse(await list.Content.ReadAsStringAsync()).RootElement.GetProperty("data").GetProperty("total").GetInt32());
        using var info = await host.Client.GetAsync($"/api/library/books/{id}"); Assert.Equal(HttpStatusCode.OK, info.StatusCode);
        using var content = await host.Client.GetAsync($"/api/library/books/{id}/content");
        Assert.Equal(bytes, await content.Content.ReadAsByteArrayAsync()); Assert.True(content.Headers.CacheControl!.NoStore);
        Assert.Equal("attachment", content.Content.Headers.ContentDisposition!.DispositionType);
        var filename = Directory.GetFiles(host.Environment.PrivateRoot).Select(Path.GetFileName).Single();
        foreach (var path in new[] { $"/App_Data/reading-private/{filename}", $"/upload/{filename}", $"/{filename}" })
        { using var response = await host.Client.GetAsync(path); Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); }
        // New client retaining the persistent cookie (no password, bearer or user row).
        using var reopened = new HttpClient { BaseAddress = host.Client.BaseAddress };
        reopened.DefaultRequestHeaders.Add("Cookie", login.cookie.Split(';')[0]);
        using var restored = await reopened.GetAsync("/api/library/auth/session"); Assert.Equal(HttpStatusCode.OK, restored.StatusCode);
        await using var scope = host.App.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Empty(await db.Users.ToListAsync());
        var session = Assert.Single(await db.ReadingSessions.ToListAsync());
        Assert.DoesNotContain(login.cookie.Split(';')[0].Split('=')[1], session.TokenHash);
        Assert.Equal(Owner, Assert.Single(await db.ReadingBooks.ToListAsync()).OwnerKey);
    }

    [Theory]
    [InlineData("/api/library/books")]
    [InlineData("/api/library/books/1")]
    [InlineData("/api/library/books/1/content")]
    [InlineData("/api/library/books/1/cover")]
    [InlineData("/api/library/books/1/progress")]
    [InlineData("/api/library/books/1/annotations")]
    [InlineData("/api/library/annotations")]
    [InlineData("/api/library/continue")]
    [InlineData("/api/library/auth/session")]
    public async Task Anonymous_AndLegacyBearer_CannotRead(string path)
    {
        await using var host = await Host.Create();
        using var anonymous = await host.Client.GetAsync(path); Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);
        host.Client.DefaultRequestHeaders.Authorization = new("Bearer", "legacy-blog-token");
        using var legacy = await host.Client.GetAsync(path); Assert.Equal(HttpStatusCode.Unauthorized, legacy.StatusCode);
        using var upload = await host.Upload(EpubTestFiles.Create()); Assert.Equal(HttpStatusCode.Unauthorized, upload.StatusCode);
        using var write = await host.Client.PutAsJsonAsync("/api/library/books/1/progress", ProgressInput());
        Assert.Equal(HttpStatusCode.Unauthorized, write.StatusCode);
        using var explain = await host.Client.PostAsJsonAsync("/api/library/ai/explain", new { quote = "text", question = "why" });
        Assert.Equal(HttpStatusCode.Unauthorized, explain.StatusCode);
    }

    [Fact]
    public async Task Annotations_CreateListAndIdempotentRetry_WorkOverHttp()
    {
        await using var host = await Host.Create();
        await host.Login();
        using var uploaded = await host.Upload(EpubTestFiles.Create());
        long id = JsonDocument.Parse(await uploaded.Content.ReadAsStringAsync()).RootElement.GetProperty("data").GetProperty("id").GetInt64();
        string requestId = Guid.NewGuid().ToString();
        object body = new
        {
            contentVersion = 1,
            cfiRange = "epubcfi(/6/2!/4/2,/1:0,/1:4)",
            chapterHref = "chapter.xhtml",
            quote = "原文片段",
            note = "我的笔记",
            kind = "note",
            color = "yellow",
            requestId
        };
        using HttpResponseMessage created = await host.Client.PostAsJsonAsync($"/api/library/books/{id}/annotations", body);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        string first = await created.Content.ReadAsStringAsync();
        using HttpResponseMessage retry = await host.Client.PostAsJsonAsync($"/api/library/books/{id}/annotations", body);
        Assert.Equal(HttpStatusCode.Created, retry.StatusCode);
        Assert.Equal(first, await retry.Content.ReadAsStringAsync());
        using HttpResponseMessage list = await host.Client.GetAsync($"/api/library/books/{id}/annotations");
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        Assert.Equal(1, JsonDocument.Parse(await list.Content.ReadAsStringAsync()).RootElement.GetProperty("data").GetArrayLength());
        using HttpResponseMessage feed = await host.Client.GetAsync("/api/library/annotations");
        Assert.Equal(HttpStatusCode.OK, feed.StatusCode);
        Assert.Equal(1, JsonDocument.Parse(await feed.Content.ReadAsStringAsync()).RootElement.GetProperty("data").GetArrayLength());
    }

    [Fact]
    public async Task DeleteAndMarkRead_WorkOverHttp()
    {
        await using var host = await Host.Create();
        await host.Login();
        using var uploaded = await host.Upload(EpubTestFiles.Create());
        long id = JsonDocument.Parse(await uploaded.Content.ReadAsStringAsync()).RootElement.GetProperty("data").GetProperty("id").GetInt64();
        using HttpResponseMessage marked = await host.Client.PostAsync($"/api/library/books/{id}/mark-read", null);
        Assert.Equal(HttpStatusCode.OK, marked.StatusCode);
        Assert.Equal(1, JsonDocument.Parse(await marked.Content.ReadAsStringAsync()).RootElement.GetProperty("data").GetProperty("progression").GetDouble());
        using HttpResponseMessage list = await host.Client.GetAsync("/api/library/books");
        Assert.Equal(1, JsonDocument.Parse(await list.Content.ReadAsStringAsync()).RootElement.GetProperty("data").GetProperty("items")[0].GetProperty("progression").GetDouble());
        using HttpResponseMessage again = await host.Client.PostAsync($"/api/library/books/{id}/mark-read", null);
        Assert.Equal(HttpStatusCode.OK, again.StatusCode);
        using HttpResponseMessage deleted = await host.Client.DeleteAsync($"/api/library/books/{id}");
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);
        using HttpResponseMessage gone = await host.Client.GetAsync($"/api/library/books/{id}");
        Assert.Equal(HttpStatusCode.NotFound, gone.StatusCode);
        using HttpResponseMessage empty = await host.Client.GetAsync("/api/library/books");
        Assert.Equal(0, JsonDocument.Parse(await empty.Content.ReadAsStringAsync()).RootElement.GetProperty("data").GetProperty("total").GetInt32());
    }

    private static object ProgressInput(long revision = 0, string cfi = "epubcfi(/6/2!/4/2/1:8)", double fraction = .6, int version = 1, string? key = null) =>
        new { contentVersion = version, cfi, chapterHref = "chapter.xhtml", progression = fraction, expectedRevision = revision, sessionId = key ?? Guid.NewGuid().ToString() };

    [Fact]
    public async Task Progress_RestoreRetryConflictAndIntentionalBackreading_WorkOverHttp()
        => await VerifyProgressOverHttp(false);

    [ReadingMySqlProgressFact]
    public async Task Progress_RealMySqlCookieHttpRoundtrip_InRolledBackTransaction()
        => await VerifyProgressOverHttp(true);

    private static async Task VerifyProgressOverHttp(bool realMySql)
    {
        await using var host = await Host.Create(realMySql: realMySql); var login = await host.Login();
        using var uploaded = await host.Upload(EpubTestFiles.Create());
        var id = JsonDocument.Parse(await uploaded.Content.ReadAsStringAsync()).RootElement.GetProperty("data").GetProperty("id").GetInt64();
        using var empty = await host.Client.GetAsync($"/api/library/books/{id}/progress");
        Assert.Equal(JsonValueKind.Null, JsonDocument.Parse(await empty.Content.ReadAsStringAsync()).RootElement.GetProperty("data").ValueKind);
        var write = ProgressInput(key: Guid.NewGuid().ToString());
        using var saved = await host.Client.PutAsJsonAsync($"/api/library/books/{id}/progress", write);
        Assert.Equal(HttpStatusCode.OK, saved.StatusCode);
        var first = await saved.Content.ReadAsStringAsync();
        using var retry = await host.Client.PutAsJsonAsync($"/api/library/books/{id}/progress", write);
        Assert.Equal(first, await retry.Content.ReadAsStringAsync());
        using var secondDevice = new HttpClient { BaseAddress = host.Client.BaseAddress };
        secondDevice.DefaultRequestHeaders.Add("Cookie", login.cookie.Split(';')[0]);
        using var restored = await secondDevice.GetAsync($"/api/library/books/{id}/progress");
        Assert.Equal(first, await restored.Content.ReadAsStringAsync());
        using var stale = await host.Client.PutAsJsonAsync($"/api/library/books/{id}/progress", ProgressInput());
        Assert.Equal(HttpStatusCode.Conflict, stale.StatusCode);
        using var backread = await host.Client.PutAsJsonAsync($"/api/library/books/{id}/progress", ProgressInput(1, "epubcfi(/6/2!/4/2/1:2)", .2));
        Assert.Equal(HttpStatusCode.OK, backread.StatusCode);
        using var continued = await host.Client.GetAsync("/api/library/continue");
        var data = JsonDocument.Parse(await continued.Content.ReadAsStringAsync()).RootElement.GetProperty("data");
        Assert.Equal(.2, data.GetProperty("progress").GetProperty("progression").GetDouble());
        Assert.Equal(2, data.GetProperty("progress").GetProperty("revision").GetInt64());
        using var list = await host.Client.GetAsync("/api/library/books");
        Assert.Equal(.2, JsonDocument.Parse(await list.Content.ReadAsStringAsync()).RootElement.GetProperty("data").GetProperty("items")[0].GetProperty("progression").GetDouble());
    }

    [Fact]
    public async Task Progress_RejectsBadLocationsVersionForeignOwnerAndMissingCsrf()
    {
        await using var host = await Host.Create(); await host.Login();
        using var uploaded = await host.Upload(EpubTestFiles.Create());
        var id = JsonDocument.Parse(await uploaded.Content.ReadAsStringAsync()).RootElement.GetProperty("data").GetProperty("id").GetInt64();
        foreach (var input in new[] { ProgressInput(cfi: "not-cfi"), ProgressInput(fraction: 1.1), ProgressInput(revision: -1) })
        { using var bad = await host.Client.PutAsJsonAsync($"/api/library/books/{id}/progress", input); Assert.Equal(HttpStatusCode.BadRequest, bad.StatusCode); }
        using var version = await host.Client.PutAsJsonAsync($"/api/library/books/{id}/progress", ProgressInput(version: 2));
        Assert.Equal(HttpStatusCode.Conflict, version.StatusCode);
        using var absent = await host.Client.PutAsJsonAsync("/api/library/books/999/progress", ProgressInput()); Assert.Equal(HttpStatusCode.NotFound, absent.StatusCode);
        await using (var scope = host.App.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.ReadingBooks.Add(new ReadingBook { Id = 99, OwnerKey = "22222222-2222-4222-8222-222222222222", Title = "Other", Sha256 = new string('b', 64), StorageKey = "other.epub" }); await db.SaveChangesAsync();
        }
        using var other = await host.Client.GetAsync("/api/library/books/99/progress"); Assert.Equal(HttpStatusCode.NotFound, other.StatusCode);
        using var otherWrite = await host.Client.PutAsJsonAsync("/api/library/books/99/progress", ProgressInput()); Assert.Equal(HttpStatusCode.NotFound, otherWrite.StatusCode);
        host.Client.DefaultRequestHeaders.Remove(ReadingSessionService.CsrfHeader);
        using var csrf = await host.Client.PutAsJsonAsync($"/api/library/books/{id}/progress", ProgressInput()); Assert.Equal(HttpStatusCode.Forbidden, csrf.StatusCode);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Cover_IsPrivateAndSupportsEpub2And3(bool legacy)
    {
        await using var host = await Host.Create();
        await host.Login();
        using var upload = await host.Upload(EpubTestFiles.WithCover(legacy));
        Assert.Equal(HttpStatusCode.Created, upload.StatusCode);
        var id = JsonDocument.Parse(await upload.Content.ReadAsStringAsync()).RootElement.GetProperty("data").GetProperty("id").GetInt64();
        using var cover = await host.Client.GetAsync($"/api/library/books/{id}/cover");
        Assert.Equal(HttpStatusCode.OK, cover.StatusCode);
        Assert.Equal("image/png", cover.Content.Headers.ContentType!.MediaType);
        Assert.True(cover.Headers.CacheControl!.NoStore);
        Assert.Equal("nosniff", cover.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.NotEmpty(await cover.Content.ReadAsByteArrayAsync());
        using var missing = await host.Client.GetAsync("/api/library/books/999/cover");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact]
    public async Task CoverlessBookReturns404()
    {
        await using var host = await Host.Create(); await host.Login();
        using var upload = await host.Upload(EpubTestFiles.Create());
        var id = JsonDocument.Parse(await upload.Content.ReadAsStringAsync()).RootElement.GetProperty("data").GetProperty("id").GetInt64();
        using var cover = await host.Client.GetAsync($"/api/library/books/{id}/cover");
        Assert.Equal(HttpStatusCode.NotFound, cover.StatusCode);
    }

    [Fact]
    public async Task ValidBlogJwt_DoesNotGrantReadingSession()
    {
        await using var host = await Host.Create();
        var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken("blog-tests", "blog-client",
            [new Claim("sub", "1"), new Claim("unique_name", "admin"), new Claim(ClaimTypes.Role, "admin")],
            expires: DateTime.UtcNow.AddHours(1), signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(LegacyKey)), SecurityAlgorithms.HmacSha256)));
        host.Client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        using var blog = await host.Client.GetAsync("/api/Auth/session");
        Assert.True(JsonDocument.Parse(await blog.Content.ReadAsStringAsync()).RootElement.GetProperty("authenticated").GetBoolean());
        using var reading = await host.Client.GetAsync("/api/library/books");
        Assert.Equal(HttpStatusCode.Unauthorized, reading.StatusCode);
    }

    [Fact]
    public async Task WrongPassword_IsRejected_AndRateLimitApplies()
    {
        await using var host = await Host.Create();
        for (var attempt = 0; attempt < 5; attempt++)
        {
            using var response = await host.Client.PostAsJsonAsync("/api/library/auth/login", new { password = "incorrect" });
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode); Assert.False(response.Headers.Contains("Set-Cookie"));
        }
        using var blocked = await host.Client.PostAsJsonAsync("/api/library/auth/login", new { password = "reading-test-password" });
        Assert.Equal(HttpStatusCode.TooManyRequests, blocked.StatusCode);
    }

    [Theory]
    [InlineData("missing-csrf")]
    [InlineData("wrong-csrf")]
    [InlineData("bad-origin")]
    [InlineData("missing-origin")]
    public async Task Writes_RequireCsrfAndAllowedOrigin(string scenario)
    {
        await using var host = await Host.Create(); await host.Login();
        if (scenario == "missing-csrf") host.Client.DefaultRequestHeaders.Remove(ReadingSessionService.CsrfHeader);
        if (scenario == "wrong-csrf") { host.Client.DefaultRequestHeaders.Remove(ReadingSessionService.CsrfHeader); host.Client.DefaultRequestHeaders.Add(ReadingSessionService.CsrfHeader, new string('a', 64)); }
        if (scenario is "bad-origin" or "missing-origin") { host.Client.DefaultRequestHeaders.Remove("Origin"); if (scenario == "bad-origin") host.Client.DefaultRequestHeaders.Add("Origin", "https://evil.test"); }
        using var upload = await host.Upload(EpubTestFiles.Create()); Assert.Equal(HttpStatusCode.Forbidden, upload.StatusCode);
        using var renew = await host.Client.PostAsync("/api/library/auth/renew", null); Assert.Equal(HttpStatusCode.Forbidden, renew.StatusCode);
        using var logout = await host.Client.PostAsync("/api/library/auth/logout", null); Assert.Equal(HttpStatusCode.Forbidden, logout.StatusCode);
    }

    [Fact]
    public async Task LoginRejectsForeignOrigin_AndCorsIsRestricted()
    {
        await using var host = await Host.Create(); host.Client.DefaultRequestHeaders.Remove("Origin"); host.Client.DefaultRequestHeaders.Add("Origin", "https://evil.test");
        using var response = await host.Client.PostAsJsonAsync("/api/library/auth/login", new { password = "reading-test-password" });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task Renewal_IsBounded_AndLogoutRevokesOriginalCookie()
    {
        await using var host = await Host.Create(); var login = await host.Login();
        await using (var scope = host.App.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var session = await db.ReadingSessions.SingleAsync();
            session.ExpiresAt = DateTime.UtcNow.AddDays(1); session.AbsoluteExpiresAt = DateTime.UtcNow.AddDays(2); await db.SaveChangesAsync();
        }
        using var renewed = await host.Client.PostAsync("/api/library/auth/renew", null); Assert.Equal(HttpStatusCode.OK, renewed.StatusCode);
        var expires = JsonDocument.Parse(await renewed.Content.ReadAsStringAsync()).RootElement.GetProperty("expiresAt").GetDateTime();
        Assert.InRange(expires, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2).AddSeconds(1));
        using var logout = await host.Client.PostAsync("/api/library/auth/logout", null); Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);
        using var after = await host.Client.GetAsync("/api/library/books"); Assert.Equal(HttpStatusCode.Unauthorized, after.StatusCode);
        using var renewAfter = await host.Client.PostAsync("/api/library/auth/renew", null); Assert.Equal(HttpStatusCode.Unauthorized, renewAfter.StatusCode);
    }

    [Theory]
    [InlineData("expired")]
    [InlineData("absolute-expired")]
    [InlineData("revoked")]
    [InlineData("other-owner")]
    [InlineData("password-changed")]
    [InlineData("tampered-cookie")]
    public async Task InvalidSession_IsRejected(string scenario)
    {
        await using var host = await Host.Create(); await host.Login();
        await using (var scope = host.App.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var s = await db.ReadingSessions.SingleAsync();
            if (scenario == "expired") s.ExpiresAt = DateTime.UtcNow.AddSeconds(-1);
            if (scenario == "absolute-expired") s.AbsoluteExpiresAt = DateTime.UtcNow.AddSeconds(-1);
            if (scenario == "revoked") s.RevokedAt = DateTime.UtcNow;
            if (scenario == "other-owner") s.OwnerKey = "22222222-2222-4222-8222-222222222222";
            if (scenario == "password-changed") s.CredentialVersion = new string('a', 64);
            await db.SaveChangesAsync();
        }
        if (scenario == "tampered-cookie") { host.Client.DefaultRequestHeaders.Remove("Cookie"); host.Client.DefaultRequestHeaders.Add("Cookie", ReadingSessionService.CookieName + "=" + new string('a', 64)); }
        using var response = await host.Client.GetAsync("/api/library/books"); Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task OtherOwnersBooks_AreHidden()
    {
        await using var host = await Host.Create(); await host.Login();
        await using (var scope = host.App.Services.CreateAsyncScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); db.ReadingBooks.Add(new ReadingBook { Id = 22, OwnerKey = "22222222-2222-4222-8222-222222222222", Title = "Other", Sha256 = new string('a',64), StorageKey = new string('a',32)+".epub" }); await db.SaveChangesAsync(); }
        foreach(var path in new[]{"/api/library/books/22","/api/library/books/22/content","/api/library/books/22/cover"}) {using var response=await host.Client.GetAsync(path);Assert.Equal(HttpStatusCode.NotFound,response.StatusCode);}
    }

    [Fact]
    public async Task MissingSecureConfiguration_FailsClosed()
    {
        await using var host = await Host.Create(configured:false);
        using var response = await host.Client.GetAsync("/api/library/books"); Assert.Equal(HttpStatusCode.ServiceUnavailable,response.StatusCode);
    }

    [Fact]
    public async Task OversizedMultipartBody_IsRejectedByHttpServer()
    {
        await using var host = await Host.Create(); await host.Login();
        using var response = await host.Upload(new byte[ReadingLibraryOptions.MaxUploadBytes + 2 * 1024 * 1024]);
        Assert.Equal(HttpStatusCode.RequestEntityTooLarge,response.StatusCode); Assert.False(Directory.Exists(host.Environment.PrivateRoot));
    }

    [Fact]
    public async Task UploadAboveOldLimit_ReachesFormatValidation()
    {
        await using var host = await Host.Create(); await host.Login();
        // Larger than the old application and default Kestrel limits. Invalid ZIP still fails safely.
        using var response = await host.Upload(new byte[30 * 1024 * 1024]);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Theory]
    [InlineData("book.epub",422)]
    [InlineData("book.pdf",422)]
    [InlineData("book.mobi",422)]
    public async Task InvalidUpload_IsRejected(string name,int status)
    {
        await using var host=await Host.Create();await host.Login();using var response=await host.Upload(Encoding.UTF8.GetBytes("not an epub"),name);
        Assert.Equal(status,(int)response.StatusCode);
    }

    private sealed class Host : IAsyncDisposable
    {
        public ReadingTestEnvironment Environment {get;} = new();
        public WebApplication App {get;private set;} = null!;
        public HttpClient Client {get;private set;} = null!;
        private AppDbContext? transactionDb;
        private IDbContextTransaction? transaction;
        public static async Task<Host> Create(bool configured=true, bool realMySql=false)
        {
            var host=new Host();
            try {
                var builder=WebApplication.CreateBuilder(new WebApplicationOptions {ContentRootPath=host.Environment.ContentRootPath,WebRootPath=host.Environment.WebRootPath});
                builder.Logging.ClearProviders();builder.WebHost.UseUrls("http://127.0.0.1:0");
                var owner = realMySql ? Guid.NewGuid().ToString() : Owner;
                builder.Configuration.AddInMemoryCollection(new Dictionary<string,string?> {
                    ["ReadingLibrary:OwnerKey"]=configured?owner:"",["ReadingLibrary:AccessPasswordHash"]=PasswordHash,
                    ["ReadingLibrary:AllowedOrigins:0"]=Origin });
                builder.Services.Configure<ReadingLibraryOptions>(builder.Configuration.GetSection("ReadingLibrary"));
                builder.Services.AddSingleton<IWebHostEnvironment>(host.Environment);
                // Stable database name shared across all requests/scopes.
                var dbName=Guid.NewGuid().ToString();
                if (realMySql)
                {
                    var config = JsonDocument.Parse(await File.ReadAllTextAsync(System.Environment.GetEnvironmentVariable("READING_MYSQL_VERIFY_CONFIG")!));
                    var connection = config.RootElement.GetProperty("ConnectionStrings").GetProperty("Default").GetString()!;
                    var options = new DbContextOptionsBuilder<AppDbContext>().UseMySql(connection, ServerVersion.Parse("8.0.36-mysql")).Options;
                    host.transactionDb = new AppDbContext(options);
                    await host.transactionDb.Database.OpenConnectionAsync();
                    host.transaction = await host.transactionDb.Database.BeginTransactionAsync();
                    builder.Services.AddDbContext<AppDbContext>(o => o.UseMySql(host.transactionDb.Database.GetDbConnection(), ServerVersion.Parse("8.0.36-mysql")));
                }
                else builder.Services.AddDbContext<AppDbContext>(o=>o.UseInMemoryDatabase(dbName));
                builder.Services.AddScoped<ReadingOwnerFilter>();builder.Services.AddSingleton<PrivateBookStorage>();builder.Services.AddSingleton<EpubImportValidator>();builder.Services.AddSingleton<CalibreBookConverter>();builder.Services.AddScoped<ReadingLibraryService>();
                builder.Services.AddHttpClient();
                builder.Services.Configure<PersonalSite.Api.Services.AiServiceOptions>(_ => { });
                builder.Services.AddScoped<ReadingCompanionAiService>();
                ReadingSecurityRegistration.AddReadingSecurity(builder.Services,builder.Configuration);
                builder.Services.AddCors(o=>o.AddPolicy("AllowAll",p=>p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
                    o.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = true, ValidateAudience = true,
                        ValidateLifetime = true, ValidateIssuerSigningKey = true, ValidIssuer = "blog-tests", ValidAudience = "blog-client",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(LegacyKey)) });
                builder.Services.AddAuthorization();
                builder.Services.AddControllers(o=>o.Filters.AddService<ReadingOwnerFilter>()).AddApplicationPart(typeof(LibraryController).Assembly);
                host.App=builder.Build();host.App.UseRouting();host.App.UseCors("AllowAll");host.App.UseRateLimiter();
                if (realMySql) host.App.Use(async (http, next) =>
                {
                    var db = http.RequestServices.GetRequiredService<AppDbContext>();
                    await db.Database.UseTransactionAsync(host.transaction!.GetDbTransaction());
                    await next();
                });
                host.App.UseStaticFiles(new StaticFileOptions {FileProvider=new PhysicalFileProvider(host.Environment.WebRootPath)});
                host.App.UseAuthentication(); host.App.UseAuthorization();
                host.App.MapControllers();await host.App.StartAsync();
                var address=host.App.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
                host.Client=new HttpClient(new HttpClientHandler {UseCookies=false}) {BaseAddress=new Uri(address)};host.Client.DefaultRequestHeaders.Add("Origin",Origin);
                return host;
            } catch {await host.DisposeAsync();throw;}
        }
        public async Task<(string cookie,string csrf)> Login()
        {
            using var response=await Client.PostAsJsonAsync("/api/library/auth/login",new{password="reading-test-password"});
            Assert.True(response.StatusCode == HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
            var cookie=response.Headers.GetValues("Set-Cookie").Single();var csrf=JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("csrfToken").GetString()!;
            Client.DefaultRequestHeaders.Remove("Cookie");Client.DefaultRequestHeaders.Add("Cookie",cookie.Split(';')[0]);
            Client.DefaultRequestHeaders.Remove(ReadingSessionService.CsrfHeader);Client.DefaultRequestHeaders.Add(ReadingSessionService.CsrfHeader,csrf);
            return(cookie,csrf);
        }
        public async Task<HttpResponseMessage> Upload(byte[] bytes,string name="book.epub")
        {
            using var form=new MultipartFormDataContent();form.Add(new ByteArrayContent(bytes),"file",name);
            using var request=new HttpRequestMessage(HttpMethod.Post,"/api/library/books"){Content=form};request.Headers.ExpectContinue=true;
            return await Client.SendAsync(request);
        }
        public async ValueTask DisposeAsync()
        {
            Client?.Dispose(); if(App!=null)await App.DisposeAsync();
            if (transaction != null) { await transaction.RollbackAsync(); await transaction.DisposeAsync(); }
            if (transactionDb != null) await transactionDb.DisposeAsync();
            Environment.Dispose();
        }
    }
}
