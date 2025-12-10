using System.Text;
using System.Text.Json;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Services;

/// <summary>
/// 内容生成智能体服务
/// 用于生成文章、项目介绍、工具介绍等内容
/// </summary>
public class ContentAgentService : AiAgentService
{
    public ContentAgentService(
        AiServiceClient aiClient,
        AppDbContext dbContext,
        ILogger<ContentAgentService> logger)
        : base(aiClient, dbContext, logger)
    {
    }

    /// <summary>
    /// 生成内容
    /// </summary>
    public async Task<ContentGenerationResult> GenerateContentAsync(
        ContentGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        // 构建 Prompt
        var prompt = BuildPrompt(request);

        // 调用 AI
        var meta = new Dictionary<string, object>
        {
            ["Type"] = request.Type ?? "",
            ["Title"] = request.Title ?? "",
            ["Tone"] = request.Tone ?? "mature",
            ["TargetAudience"] = request.TargetAudience ?? "",
            ["Length"] = request.Length ?? "medium"
        };

        var aiResponse = await CallAiAsync("Content", prompt, meta, cancellationToken);

        // 解析响应（假设 AI 返回 JSON 格式）
        var result = ParseAiResponse(aiResponse, request);

        // 如果设置了自动保存草稿，则保存到数据库
        if (request.AutoSaveDraft == true)
        {
            await SaveDraftAsync(result, request, cancellationToken);
        }

        return result;
    }

    /// <summary>
    /// 构建 Prompt
    /// </summary>
    private string BuildPrompt(ContentGenerationRequest request)
    {
        var sb = new StringBuilder();

        // 根据类型设置不同的提示词
        switch (request.Type?.ToLower())
        {
            case "article":
                sb.AppendLine("请生成一篇技术文章，要求如下：");
                break;
            case "project_intro":
                sb.AppendLine("请生成一个项目的介绍文案，要求如下：");
                break;
            case "tool_intro":
                sb.AppendLine("请生成一个工具的介绍文案，要求如下：");
                break;
            default:
                sb.AppendLine("请生成内容，要求如下：");
                break;
        }

        if (!string.IsNullOrEmpty(request.Title))
        {
            sb.AppendLine($"标题：{request.Title}");
        }

        sb.AppendLine($"语气风格：{request.Tone ?? "mature"}（mature=成熟专业，casual=轻松随意，technical=技术导向）");
        sb.AppendLine($"目标受众：{request.TargetAudience ?? "通用"}");
        sb.AppendLine($"字数要求：{request.Length ?? "medium"}（short=500字左右，medium=1000字左右，long=2000字以上）");

        if (!string.IsNullOrEmpty(request.ExtraNotes))
        {
            sb.AppendLine($"额外说明：{request.ExtraNotes}");
        }

        sb.AppendLine();
        sb.AppendLine("请以 JSON 格式返回，包含以下字段：");
        sb.AppendLine("- title: 标题");
        sb.AppendLine("- summary: 摘要（100-200字）");
        sb.AppendLine("- body: 正文内容（Markdown 格式）");

        return sb.ToString();
    }

    /// <summary>
    /// 解析 AI 响应
    /// </summary>
    private ContentGenerationResult ParseAiResponse(string aiResponse, ContentGenerationRequest request)
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

                return new ContentGenerationResult
                {
                    Success = true,
                    Content = new ContentDto
                    {
                        Title = root.TryGetProperty("title", out var title) ? title.GetString() ?? request.Title ?? "未命名" : request.Title ?? "未命名",
                        Summary = root.TryGetProperty("summary", out var summary) ? summary.GetString() ?? "" : "",
                        Body = root.TryGetProperty("body", out var body) ? body.GetString() ?? "" : ""
                    }
                };
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "解析 AI 响应 JSON 失败，使用原始文本");
        }

        // 如果解析失败，使用原始文本
        return new ContentGenerationResult
        {
            Success = true,
            Content = new ContentDto
            {
                Title = request.Title ?? "未命名",
                Summary = "",
                Body = aiResponse
            }
        };
    }

    /// <summary>
    /// 保存草稿到数据库
    /// </summary>
    private async Task SaveDraftAsync(
        ContentGenerationResult result,
        ContentGenerationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (result.Content == null)
            {
                Logger.LogWarning("内容生成结果为空，无法保存草稿");
                return;
            }

            var article = new Article
            {
                Title = result.Content.Title,
                Summary = result.Content.Summary,
                ContentMd = result.Content.Body,
                Status = 0, // 草稿状态
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            DbContext.Articles.Add(article);
            await DbContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation("内容已保存为草稿: ArticleId={ArticleId}", article.Id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "保存草稿失败");
            // 不抛出异常，避免影响主流程
        }
    }
}

/// <summary>
/// 内容生成请求 DTO
/// </summary>
public class ContentGenerationRequest
{
    /// <summary>
    /// 内容类型：article（文章）、project_intro（项目介绍）、tool_intro（工具介绍）
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 标题（可选）
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 语气：mature（成熟专业）、casual（轻松随意）、technical（技术导向）
    /// </summary>
    public string? Tone { get; set; }

    /// <summary>
    /// 目标受众
    /// </summary>
    public string? TargetAudience { get; set; }

    /// <summary>
    /// 字数：short（500字左右）、medium（1000字左右）、long（2000字以上）
    /// </summary>
    public string? Length { get; set; }

    /// <summary>
    /// 额外说明
    /// </summary>
    public string? ExtraNotes { get; set; }

    /// <summary>
    /// 是否自动保存为草稿
    /// </summary>
    public bool? AutoSaveDraft { get; set; }
}

/// <summary>
/// 内容生成响应 DTO
/// </summary>
public class ContentGenerationResult
{
    public bool Success { get; set; }
    public ContentDto? Content { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// 生成的内容 DTO
/// </summary>
public class ContentDto
{
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty; // Markdown 格式
}

