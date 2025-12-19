using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PersonalSite.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器

// 1. 配置数据库 (MySQL)
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySqlOptions =>
    {
        // 启用重试机制，处理瞬时连接失败
        mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,                    // 最大重试次数
            maxRetryDelay: TimeSpan.FromSeconds(30), // 最大重试延迟
            errorNumbersToAdd: null);            // 要添加的错误代码列表（null 表示使用默认值）
        
        // 设置命令超时时间（秒）
        mySqlOptions.CommandTimeout(60);
    })
    // 注意：以下配置仅在开发环境启用，生产环境会自动禁用
    // 开发环境中可能会看到敏感数据日志警告，这是正常的预期行为
    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()) // 仅在开发环境启用敏感数据日志（用于调试）
    .EnableDetailedErrors(builder.Environment.IsDevelopment()));     // 仅在开发环境启用详细错误（用于调试）

// 2. 配置 JWT 认证
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyHere_MustBeAtLeast32BytesLong";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// 3. 配置 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient(); // 添加 HttpClient 支持
builder.Services.AddHttpContextAccessor(); // 添加 HttpContextAccessor 支持

// 配置 ForwardedHeaders（修复线上访问仍然显示未知的问题：支持从 Nginx 反向代理获取真实客户端 IP）
// 注意：X-Real-IP 不是标准的 ForwardedHeaders 枚举值，已在 AnalyticsController 中手动处理
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // 信任所有代理（生产环境建议配置具体的代理 IP）
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// 配置内存缓存（用于限流）
builder.Services.AddMemoryCache();

// 注册内存缓存服务（Singleton，因为 IMemoryCache 是 Singleton）
builder.Services.AddSingleton<PersonalSite.Api.Services.Cache.MemoryCacheService>();

// 注册 Redis 缓存服务（Singleton，即使未启用也会注册，但不会使用）
builder.Services.AddSingleton<PersonalSite.Api.Services.Cache.RedisCacheService>();

// 配置缓存服务（支持内存缓存和Redis）- 使用 Singleton，因为中间件需要
builder.Services.AddSingleton<PersonalSite.Api.Services.Cache.ICacheService>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var redisEnabled = configuration.GetValue<bool>("Redis:Enabled", false);
    
    if (redisEnabled)
    {
        // 使用 Redis 缓存服务
        return serviceProvider.GetRequiredService<PersonalSite.Api.Services.Cache.RedisCacheService>();
    }
    else
    {
        // 使用内存缓存服务
        return serviceProvider.GetRequiredService<PersonalSite.Api.Services.Cache.MemoryCacheService>();
    }
});

// 配置限流选项
builder.Services.Configure<PersonalSite.Api.Middleware.RateLimitOptions>(options =>
{
    options.DefaultLimit = builder.Configuration.GetValue<int>("RateLimit:DefaultLimit", 100);
    options.WindowSeconds = builder.Configuration.GetValue<int>("RateLimit:WindowSeconds", 60);
});

// 配置 AI Service 客户端
builder.Services.Configure<PersonalSite.Api.Services.AiServiceOptions>(
    builder.Configuration.GetSection("AiService"));
builder.Services.AddHttpClient<PersonalSite.Api.Services.AiServiceClient>();

// 配置 NameTool AI Service（使用独立的 HttpClient）
builder.Services.AddHttpClient<PersonalSite.Api.Services.INameToolAiService, PersonalSite.Api.Services.NameToolAiService>();

// 注册 AI 智能体服务
builder.Services.AddScoped<PersonalSite.Api.Services.ContentAgentService>();
builder.Services.AddScoped<PersonalSite.Api.Services.DemoAgentService>();
builder.Services.AddScoped<PersonalSite.Api.Services.LeadAgentService>();
builder.Services.AddScoped<PersonalSite.Api.Services.SupportAgentService>();
builder.Services.AddScoped<PersonalSite.Api.Services.PersonalAssistantService>();
builder.Services.AddScoped<PersonalSite.Api.Services.QuotationAgentService>();
builder.Services.AddScoped<PersonalSite.Api.Services.INameToolAiService, PersonalSite.Api.Services.NameToolAiService>();

// 注册支付服务
builder.Services.AddScoped<PersonalSite.Api.Services.Payment.WeChatPaymentService>();
builder.Services.AddScoped<PersonalSite.Api.Services.Payment.AlipayPaymentService>();
builder.Services.AddScoped<PersonalSite.Api.Services.Payment.StripePaymentService>();
builder.Services.AddScoped<PersonalSite.Api.Services.Payment.PaymentServiceFactory>();

// 注册副业项目服务
builder.Services.AddScoped<PersonalSite.Api.Services.SideProjectService>();

// 注册通知服务
builder.Services.AddScoped<PersonalSite.Api.Services.NotificationService>();

// 注册通知生成后台服务（定时任务）
builder.Services.AddHostedService<PersonalSite.Api.Services.NotificationBackgroundService>();

// 注册数据分析服务
builder.Services.AddScoped<PersonalSite.Api.Services.SideProjectAnalyticsService>();

// 4. 配置 Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PersonalSite API", Version = "v1" });
    
    // 添加 JWT 认证支持
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// 使用 ForwardedHeaders 中间件（修复线上访问仍然显示未知的问题：必须在 UseCors 之前）
app.UseForwardedHeaders();

app.UseCors("AllowAll");

// 注册中间件（顺序很重要）
app.UseMiddleware<PersonalSite.Api.Middleware.ApiKeyAuthMiddleware>(); // API Key 验证
app.UseMiddleware<PersonalSite.Api.Middleware.RateLimitMiddleware>(); // 限流

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 健康检查接口
app.MapGet("/api/health", () => "Healthy");

app.Run();
