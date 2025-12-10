using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Services;

/// <summary>
/// 客服问答智能体服务
/// 为网站访客提供智能客服问答功能
/// </summary>
public class SupportAgentService : AiAgentService
{
    public SupportAgentService(
        AiServiceClient aiClient,
        AppDbContext dbContext,
        ILogger<SupportAgentService> logger)
        : base(aiClient, dbContext, logger)
    {
    }

    /// <summary>
    /// 回答客服问题
    /// </summary>
    public async Task<SupportAnswerResult> AnswerQuestionAsync(
        SupportAnswerRequest request,
        CancellationToken cancellationToken = default)
    {
        // 读取系统配置
        var systemPrompt = await GetSystemPromptAsync(cancellationToken);
        var faqList = await GetFaqListAsync(cancellationToken);

        // 构建 Prompt
        var prompt = BuildPrompt(request, systemPrompt, faqList);

        // 调用 AI
        var meta = new Dictionary<string, object>
        {
            ["Category"] = request.Category ?? "general",
            ["PageContext"] = request.PageContext ?? "",
            ["UserMeta"] = request.UserMeta ?? ""
        };

        var aiResponse = await CallAiAsync("Support", prompt, meta, cancellationToken);

        // 解析响应
        var result = ParseAiResponse(aiResponse, request);

        return result;
    }

    /// <summary>
    /// 获取系统提示词
    /// </summary>
    private async Task<string> GetSystemPromptAsync(CancellationToken cancellationToken)
    {
        var config = await DbContext.SupportConfigs
            .FirstOrDefaultAsync(c => c.ConfigKey == "system_prompt", cancellationToken);

        if (config != null && !string.IsNullOrEmpty(config.ConfigValue))
        {
            return config.ConfigValue;
        }

        // 默认提示词
        return "你是溪午听风个人网站的智能客服助手。你的任务是友好、专业地回答访客的问题，帮助他们了解服务内容、项目开发、工具使用等信息。请保持简洁明了的回答风格。";
    }

    /// <summary>
    /// 获取 FAQ 列表
    /// </summary>
    private async Task<List<FaqItem>> GetFaqListAsync(CancellationToken cancellationToken)
    {
        var config = await DbContext.SupportConfigs
            .FirstOrDefaultAsync(c => c.ConfigKey == "faq_list", cancellationToken);

        if (config != null && !string.IsNullOrEmpty(config.ConfigValue))
        {
            try
            {
                var faqList = JsonSerializer.Deserialize<List<FaqItem>>(config.ConfigValue);
                return faqList ?? new List<FaqItem>();
            }
            catch
            {
                return new List<FaqItem>();
            }
        }

        return new List<FaqItem>();
    }

    /// <summary>
    /// 构建 Prompt
    /// </summary>
    private string BuildPrompt(SupportAnswerRequest request, string systemPrompt, List<FaqItem> faqList)
    {
        var sb = new StringBuilder();

        // 系统提示词
        sb.AppendLine(systemPrompt);
        sb.AppendLine();

        // 添加上下文信息
        if (!string.IsNullOrEmpty(request.PageContext))
        {
            sb.AppendLine($"用户当前所在页面：{request.PageContext}");
        }

        if (!string.IsNullOrEmpty(request.UserMeta))
        {
            sb.AppendLine($"用户信息：{request.UserMeta}");
        }

        // 添加 FAQ 作为参考
        if (faqList.Any())
        {
            sb.AppendLine();
            sb.AppendLine("以下是常见问题参考（如果用户问题与这些相关，可以参考回答）：");
            foreach (var faq in faqList)
            {
                sb.AppendLine($"Q: {faq.Question}");
                sb.AppendLine($"A: {faq.Answer}");
                sb.AppendLine();
            }
        }

        // 用户问题
        sb.AppendLine();
        sb.AppendLine($"用户问题：{request.Question}");
        sb.AppendLine();
        sb.AppendLine("请以 JSON 格式返回，包含以下字段：");
        sb.AppendLine("- answer: 回答内容（Markdown 格式）");
        sb.AppendLine("- needHuman: 是否需要人工跟进（布尔值）");
        sb.AppendLine("- relatedLinks: 相关链接列表（数组，格式：[\"链接文本|链接地址\"])");

        return sb.ToString();
    }

    /// <summary>
    /// 解析 AI 响应
    /// </summary>
    private SupportAnswerResult ParseAiResponse(string aiResponse, SupportAnswerRequest request)
    {
        try
        {
            // 尝试解析 JSON
            var jsonStart = aiResponse.IndexOf('{');
            var jsonEnd = aiResponse.LastIndexOf('}');
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonStr = aiResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
                var jsonDoc = JsonDocument.Parse(jsonStr);
                var root = jsonDoc.RootElement;

                var relatedLinks = new List<string>();
                if (root.TryGetProperty("relatedLinks", out var linksEl) && linksEl.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in linksEl.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                        {
                            relatedLinks.Add(item.GetString() ?? "");
                        }
                    }
                }

                return new SupportAnswerResult
                {
                    Success = true,
                    Answer = root.TryGetProperty("answer", out var answer) ? answer.GetString() ?? "" : "",
                    RelatedLinks = relatedLinks,
                    NeedHuman = root.TryGetProperty("needHuman", out var needHuman) && needHuman.GetBoolean(),
                    DebugInfo = null // 生产环境不返回调试信息
                };
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "解析 AI 响应 JSON 失败，使用原始文本");
        }

        // 如果解析失败，使用原始文本
        return new SupportAnswerResult
        {
            Success = true,
            Answer = aiResponse,
            RelatedLinks = new List<string>(),
            NeedHuman = false
        };
    }
}

/// <summary>
/// 客服问答请求 DTO
/// </summary>
public class SupportAnswerRequest
{
    /// <summary>
    /// 用户问题
    /// </summary>
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// 问题分类（general / service / pricing / tool）
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 页面上下文（用户提问时所在页面的标识）
    /// </summary>
    public string? PageContext { get; set; }

    /// <summary>
    /// 用户元信息（如"未登录访客 / 微信扫码过来"）
    /// </summary>
    public string? UserMeta { get; set; }
}

/// <summary>
/// 客服问答响应 DTO
/// </summary>
public class SupportAnswerResult
{
    public bool Success { get; set; }
    public string Answer { get; set; } = string.Empty;
    public List<string> RelatedLinks { get; set; } = new();
    public bool NeedHuman { get; set; }
    public string? DebugInfo { get; set; }
}

