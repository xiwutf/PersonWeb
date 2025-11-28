using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Text;
using System.Text.Json;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public RecommendationController(AppDbContext context, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// 记录用户行为
    /// </summary>
    [HttpPost("behavior")]
    public async Task<ActionResult<ApiResponse>> RecordBehavior([FromBody] RecommendationBehaviorRequest request)
    {
        try
        {
            var behavior = new UserBehavior
            {
                UserId = request.UserId,
                BehaviorType = request.BehaviorType,
                TargetId = request.TargetId,
                TargetTitle = request.TargetTitle,
                Tags = request.Tags,
                Category = request.Category,
                Duration = request.Duration,
                CreatedAt = DateTime.Now
            };

            _context.UserBehaviors.Add(behavior);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(null, "记录成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"记录失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取 AI 推荐
    /// </summary>
    [HttpGet("recommendations")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetRecommendations([FromQuery] string type = "all")
    {
        try
        {
            // 获取用户行为数据
            var behaviors = await _context.UserBehaviors
                .Where(b => b.UserId != null)
                .OrderByDescending(b => b.CreatedAt)
                .Take(100)
                .ToListAsync();

            // 分析用户偏好
            var preferences = AnalyzePreferences(behaviors);

            // 调用 AI 生成推荐
            var recommendations = await GenerateRecommendations(preferences, type);

            return Ok(ApiResponse.Success(recommendations));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"获取推荐失败: {ex.Message}", 500));
        }
    }

    private Dictionary<string, object> AnalyzePreferences(List<UserBehavior> behaviors)
    {
        var preferences = new Dictionary<string, object>
        {
            ["favorite_categories"] = behaviors
                .Where(b => !string.IsNullOrEmpty(b.Category))
                .GroupBy(b => b.Category)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToList(),
            ["favorite_tags"] = behaviors
                .Where(b => !string.IsNullOrEmpty(b.Tags))
                .SelectMany(b => b.Tags!.Split(','))
                .GroupBy(t => t.Trim())
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => g.Key)
                .ToList(),
            ["read_articles"] = behaviors
                .Where(b => b.BehaviorType == "read_article")
                .Select(b => new { b.TargetTitle, b.Category, b.Tags })
                .ToList(),
            ["total_behaviors"] = behaviors.Count
        };

        return preferences;
    }

    private async Task<object> GenerateRecommendations(Dictionary<string, object> preferences, string type)
    {
        var apiKey = _configuration["DeepSeek:ApiKey"] ?? _configuration["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            return new { message = "AI 服务未配置" };
        }

        var prompt = BuildRecommendationPrompt(preferences, type);
        var apiBaseUrl = _configuration["DeepSeek:ApiBaseUrl"] ?? "https://api.deepseek.com";
        var model = _configuration["DeepSeek:Model"] ?? "deepseek-chat";

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var payload = new
        {
            model = model,
            messages = new[]
            {
                new { role = "system", content = "你是一个智能推荐助手，根据用户的行为数据提供个性化推荐。" },
                new { role = "user", content = prompt }
            },
            temperature = 0.7,
            max_tokens = 1000
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{apiBaseUrl}/v1/chat/completions", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
            if (result.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
            {
                var aiResponse = choices[0].GetProperty("message").GetProperty("content").GetString();
                return new { recommendations = aiResponse };
            }
        }

        return new { message = "AI 推荐生成失败" };
    }

    private string BuildRecommendationPrompt(Dictionary<string, object> preferences, string type)
    {
        var prompt = new StringBuilder();
        prompt.AppendLine("根据以下用户行为数据，生成个性化推荐：");
        prompt.AppendLine();
        prompt.AppendLine($"用户偏好分类: {string.Join(", ", (preferences["favorite_categories"] as List<string>) ?? new List<string>())}");
        prompt.AppendLine($"用户偏好标签: {string.Join(", ", (preferences["favorite_tags"] as List<string>) ?? new List<string>())}");
        prompt.AppendLine($"总行为数: {preferences["total_behaviors"]}");
        prompt.AppendLine();

        switch (type)
        {
            case "books":
                prompt.AppendLine("请推荐3-5本适合该用户的书籍，并说明推荐理由。");
                break;
            case "articles":
                prompt.AppendLine("请推荐3-5篇该用户可能感兴趣的文章主题，并说明推荐理由。");
                break;
            case "features":
                prompt.AppendLine("请推荐3-5个该用户网站可以新增的功能，并说明推荐理由。");
                break;
            default:
                prompt.AppendLine("请综合推荐：");
                prompt.AppendLine("1. 3-5本适合的书籍");
                prompt.AppendLine("2. 3-5个文章主题");
                prompt.AppendLine("3. 3-5个网站功能建议");
                break;
        }

        return prompt.ToString();
    }
}

public class RecommendationBehaviorRequest
{
    public string? UserId { get; set; }
    public string BehaviorType { get; set; } = string.Empty;
    public string? TargetId { get; set; }
    public string? TargetTitle { get; set; }
    public string? Tags { get; set; }
    public string? Category { get; set; }
    public int? Duration { get; set; }
}

