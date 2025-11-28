using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Text;
using System.Text.Json;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public AIController(AppDbContext context, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// AI 聊天接口
    /// </summary>
    [HttpPost("chat")]
    public async Task<ActionResult<ApiResponse<object>>> Chat([FromBody] ChatRequest request)
    {
        try
        {
            // 支持 DeepSeek 和 OpenAI
            var apiKey = _configuration["DeepSeek:ApiKey"] ?? _configuration["OpenAI:ApiKey"];
            var apiBaseUrl = _configuration["DeepSeek:ApiBaseUrl"] ?? "https://api.deepseek.com";
            var model = _configuration["DeepSeek:Model"] ?? _configuration["OpenAI:Model"] ?? "deepseek-chat";
            
            if (string.IsNullOrEmpty(apiKey))
            {
                return Ok(ApiResponse.Error("AI 服务未配置，请在 appsettings.json 中配置 DeepSeek:ApiKey 或 OpenAI:ApiKey", 500));
            }

            // 构建系统提示词
            var systemPrompt = BuildSystemPrompt();

            // 构建消息列表
            var messages = new List<object>
            {
                new { role = "system", content = systemPrompt }
            };

            // 添加历史消息
            if (request.History != null && request.History.Count > 0)
            {
                foreach (var msg in request.History)
                {
                    messages.Add(new { role = msg.Role, content = msg.Content });
                }
            }

            // 添加当前消息
            messages.Add(new { role = "user", content = request.Message });

            // 调用 DeepSeek 或 OpenAI API
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var payload = new
            {
                model = model,
                messages = messages,
                temperature = 0.7,
                max_tokens = 1000,
                stream = false
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // DeepSeek API 端点
            var apiUrl = $"{apiBaseUrl}/v1/chat/completions";
            var response = await client.PostAsync(apiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                if (result.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var aiMessage = choices[0].GetProperty("message").GetProperty("content").GetString();
                    return Ok(ApiResponse.Success(new { message = aiMessage }));
                }
                else
                {
                    return Ok(ApiResponse.Error("AI 服务返回格式异常", 500));
                }
            }
            else
            {
                return Ok(ApiResponse.Error($"AI 服务调用失败: {responseContent}", 500));
            }
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"AI 服务错误: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 构建系统提示词
    /// </summary>
    private string BuildSystemPrompt()
    {
        // 获取网站信息（异步操作需要在调用前完成）
        var articles = _context.Articles
            .Include(a => a.Category)
            .Where(a => a.Status == 1)
            .OrderByDescending(a => a.PublishTime)
            .Take(10)
            .ToList();
        var projects = _context.Projects
            .Where(p => p.Status == "Active" || p.Status == "1")
            .Take(10)
            .ToList();

        var prompt = new StringBuilder();
        prompt.AppendLine("你是溪午听风的个人网站的 AI 智能助手，名字叫'小智'。");
        prompt.AppendLine("你的任务是帮助访客了解溪午听风，推荐文章和项目，回答问题。");
        prompt.AppendLine();
        prompt.AppendLine("关于溪午听风：");
        prompt.AppendLine("- 全栈开发者、AI应用探索者、Revit插件专家");
        prompt.AppendLine("- 专注于构建高效、优雅的数字体验");
        prompt.AppendLine();
        
        if (articles.Any())
        {
            prompt.AppendLine("最近的文章：");
            foreach (var article in articles)
            {
                prompt.AppendLine($"- {article.Title} (分类: {article.Category?.Name ?? "未分类"})");
            }
            prompt.AppendLine();
        }

        if (projects.Any())
        {
            prompt.AppendLine("推荐的项目：");
            foreach (var project in projects)
            {
                prompt.AppendLine($"- {project.Title}: {project.Description}");
            }
            prompt.AppendLine();
        }

        prompt.AppendLine("请用友好、专业的语气回答问题，保持简洁明了。");

        return prompt.ToString();
    }
}

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public List<ChatMessage>? History { get; set; }
}

public class ChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

