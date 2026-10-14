using System.Threading.RateLimiting;

namespace PersonalSite.Api.Services.Reading;

public static class ReadingSecurityRegistration
{
    public static void AddReadingSecurity(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ReadingSessionService>();
        services.AddCors(options => options.AddPolicy("ReadingOrigins", policy => {
            var origins = configuration.GetSection("ReadingLibrary:AllowedOrigins").Get<string[]>() ?? [];
            policy.WithOrigins(origins).WithMethods("GET", "HEAD", "POST", "PUT", "PATCH", "DELETE")
                .WithHeaders("Content-Type", ReadingSessionService.CsrfHeader).AllowCredentials();
        }));
        services.AddRateLimiter(options => {
            options.RejectionStatusCode = 429;
            options.OnRejected = async (context, ct) => {
                context.HttpContext.Response.Headers.CacheControl = "private, no-store";
                context.HttpContext.Response.Headers.RetryAfter = "60";
                await context.HttpContext.Response.WriteAsJsonAsync(new { error = "too_many_attempts" }, ct);
            };
            // Global password-entry budget for this single-owner library. Not affected by spoofed proxy IPs.
            options.AddPolicy("ReadingPassword", _ => RateLimitPartition.GetFixedWindowLimiter("reading-password",
                _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0, AutoReplenishment = true }));
        });
    }
}
