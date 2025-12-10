using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Services;

/// <summary>
/// Demo 上架智能体服务
/// 用于为项目/工具生成展示文案
/// </summary>
public class DemoAgentService : AiAgentService
{
    public DemoAgentService(
        AiServiceClient aiClient,
        AppDbContext dbContext,
        ILogger<DemoAgentService> logger)
        : base(aiClient, dbContext, logger)
    {
    }

    /// <summary>
    /// 生成 Demo 描述
    /// </summary>
    public async Task<DemoDescriptionResult> GenerateDescriptionAsync(
        DemoDescriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        // 读取项目/工具基础信息
        Project? project = null;
        Tool? tool = null;

        if (request.ProjectId.HasValue)
        {
            project = await DbContext.Projects
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId.Value, cancellationToken);
            
            if (project == null)
            {
                throw new Exception($"项目不存在: {request.ProjectId}");
            }
        }
        else if (request.ToolId.HasValue)
        {
            tool = await DbContext.Tools
                .FirstOrDefaultAsync(t => t.Id == request.ToolId.Value, cancellationToken);
            
            if (tool == null)
            {
                throw new Exception($"工具不存在: {request.ToolId}");
            }
        }
        else
        {
            throw new Exception("必须提供 ProjectId 或 ToolId");
        }

        // 构建 Prompt
        var prompt = BuildPrompt(request, project, tool);

        // 调用 AI
        var meta = new Dictionary<string, object>
        {
            ["ProjectId"] = request.ProjectId?.ToString() ?? "",
            ["ToolId"] = request.ToolId?.ToString() ?? "",
            ["Name"] = request.Name ?? ""
        };

        var aiResponse = await CallAiAsync("Demo", prompt, meta, cancellationToken);

        // 解析响应
        var result = ParseAiResponse(aiResponse);

        // 写回数据库
        if (project != null)
        {
            project.AiTitle = result.Title;
            project.AiHighlights = JsonSerializer.Serialize(result.Highlights);
            project.AiDescription = result.Features; // 使用 Features 作为描述内容
            project.AiScenarios = result.Scenarios;
            project.AiTargetUsers = result.TargetUsers;
            project.AiShortCardText = result.ShortCardText;
            project.UpdatedAt = DateTime.Now;
        }
        else if (tool != null)
        {
            tool.AiTitle = result.Title;
            tool.AiHighlights = JsonSerializer.Serialize(result.Highlights);
            tool.AiDescription = result.Features; // 使用 Features 作为描述内容
            tool.AiScenarios = result.Scenarios;
            tool.AiTargetUsers = result.TargetUsers;
            tool.AiShortCardText = result.ShortCardText;
            tool.UpdatedAt = DateTime.Now;
        }

        await DbContext.SaveChangesAsync(cancellationToken);

        return new DemoDescriptionResult
        {
            Success = true,
            DemoDescription = result
        };
    }

    /// <summary>
    /// 构建 Prompt
    /// </summary>
    private string BuildPrompt(DemoDescriptionRequest request, Project? project, Tool? tool)
    {
        var sb = new StringBuilder();
        sb.AppendLine("请为一个项目/工具生成展示文案，要求如下：");

        var name = request.Name ?? (project?.Title ?? tool?.Name ?? "");
        var techStack = request.TechStack?.Any() == true 
            ? string.Join(", ", request.TechStack) 
            : (project?.TechStack ?? "");
        var usage = request.Usage ?? (project?.Description ?? tool?.Description ?? "");
        var targetAudience = request.TargetAudience ?? "";
        var priceHint = request.PriceHint ?? "";

        sb.AppendLine($"名称：{name}");
        if (!string.IsNullOrEmpty(techStack))
        {
            sb.AppendLine($"技术栈：{techStack}");
        }
        if (!string.IsNullOrEmpty(usage))
        {
            sb.AppendLine($"用途：{usage}");
        }
        if (!string.IsNullOrEmpty(targetAudience))
        {
            sb.AppendLine($"目标用户：{targetAudience}");
        }
        if (!string.IsNullOrEmpty(priceHint))
        {
            sb.AppendLine($"价格提示：{priceHint}");
        }
        if (!string.IsNullOrEmpty(request.ExtraNotes))
        {
            sb.AppendLine($"额外说明：{request.ExtraNotes}");
        }

        sb.AppendLine();
        sb.AppendLine("请以 JSON 格式返回，包含以下字段：");
        sb.AppendLine("- title: 标题（吸引人的标题）");
        sb.AppendLine("- highlights: 亮点列表（数组，3-5个亮点）");
        sb.AppendLine("- features: 核心功能特性（文本描述）");
        sb.AppendLine("- scenarios: 使用场景（文本描述）");
        sb.AppendLine("- targetUsers: 目标用户（文本描述）");
        sb.AppendLine("- shortCardText: 卡片短文本（50字以内，用于卡片展示）");

        return sb.ToString();
    }

    /// <summary>
    /// 解析 AI 响应
    /// </summary>
    private DemoDescriptionDto ParseAiResponse(string aiResponse)
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

                var highlights = new List<string>();
                if (root.TryGetProperty("highlights", out var highlightsEl) && highlightsEl.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in highlightsEl.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                        {
                            highlights.Add(item.GetString() ?? "");
                        }
                    }
                }

                return new DemoDescriptionDto
                {
                    Title = root.TryGetProperty("title", out var title) ? title.GetString() ?? "" : "",
                    Highlights = highlights,
                    Features = root.TryGetProperty("features", out var features) ? features.GetString() ?? "" : "",
                    Scenarios = root.TryGetProperty("scenarios", out var scenarios) ? scenarios.GetString() ?? "" : "",
                    TargetUsers = root.TryGetProperty("targetUsers", out var targetUsers) ? targetUsers.GetString() ?? "" : "",
                    ShortCardText = root.TryGetProperty("shortCardText", out var shortCardText) ? shortCardText.GetString() ?? "" : ""
                };
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "解析 AI 响应 JSON 失败，使用默认值");
        }

        // 如果解析失败，返回默认值
        return new DemoDescriptionDto
        {
            Title = "",
            Highlights = new List<string>(),
            Features = "",
            Scenarios = "",
            TargetUsers = "",
            ShortCardText = ""
        };
    }
}

/// <summary>
/// Demo 描述生成请求 DTO
/// </summary>
public class DemoDescriptionRequest
{
    /// <summary>
    /// 项目 ID（与 ToolId 二选一）
    /// </summary>
    public Guid? ProjectId { get; set; }

    /// <summary>
    /// 工具 ID（与 ProjectId 二选一）
    /// </summary>
    public long? ToolId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 技术栈列表
    /// </summary>
    public List<string>? TechStack { get; set; }

    /// <summary>
    /// 用途
    /// </summary>
    public string? Usage { get; set; }

    /// <summary>
    /// 目标用户
    /// </summary>
    public string? TargetAudience { get; set; }

    /// <summary>
    /// 价格提示
    /// </summary>
    public string? PriceHint { get; set; }

    /// <summary>
    /// 额外说明
    /// </summary>
    public string? ExtraNotes { get; set; }
}

/// <summary>
/// Demo 描述生成响应 DTO
/// </summary>
public class DemoDescriptionResult
{
    public bool Success { get; set; }
    public DemoDescriptionDto? DemoDescription { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Demo 描述 DTO
/// </summary>
public class DemoDescriptionDto
{
    public string Title { get; set; } = string.Empty;
    public List<string> Highlights { get; set; } = new();
    public string Features { get; set; } = string.Empty;
    public string Scenarios { get; set; } = string.Empty;
    public string TargetUsers { get; set; } = string.Empty;
    public string ShortCardText { get; set; } = string.Empty;
}

