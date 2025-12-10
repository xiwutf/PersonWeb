using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Services;

/// <summary>
/// 个人助理智能体服务
/// 为管理员提供个人 AI 助理功能
/// </summary>
public class PersonalAssistantService : AiAgentService
{
    public PersonalAssistantService(
        AiServiceClient aiClient,
        AppDbContext dbContext,
        ILogger<PersonalAssistantService> logger)
        : base(aiClient, dbContext, logger)
    {
    }

    /// <summary>
    /// 个人助理对话
    /// </summary>
    public async Task<AssistantChatResult> ChatAsync(
        AssistantChatRequest request,
        CancellationToken cancellationToken = default)
    {
        AssistantSession? session = null;
        List<AssistantMessage> historyMessages = new();

        // 获取或创建会话
        if (request.SessionId.HasValue)
        {
            session = await DbContext.AssistantSessions
                .Include(s => s.Messages.OrderBy(m => m.CreatedAt))
                .FirstOrDefaultAsync(s => s.Id == request.SessionId.Value && s.UserId == request.UserId, cancellationToken);

            if (session == null)
            {
                throw new Exception($"会话不存在: {request.SessionId}");
            }

            historyMessages = session.Messages.ToList();
        }
        else
        {
            // 创建新会话
            session = new AssistantSession
            {
                UserId = request.UserId,
                Title = GenerateSessionTitle(request.Message),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            DbContext.AssistantSessions.Add(session);
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        // 保存用户消息
        var userMessage = new AssistantMessage
        {
            SessionId = session.Id,
            Role = "User",
            Content = request.Message,
            CreatedAt = DateTime.Now
        };
        DbContext.AssistantMessages.Add(userMessage);
        await DbContext.SaveChangesAsync(cancellationToken);

        // 构建 Prompt（包含上下文和系统信息）
        var prompt = BuildPrompt(request, historyMessages);

        // 调用 AI
        var meta = new Dictionary<string, object>
        {
            ["SessionId"] = session.Id.ToString(),
            ["UserId"] = request.UserId.ToString(),
            ["ContextType"] = request.ContextType ?? "general"
        };

        var aiResponse = await CallAiAsync("Assistant", prompt, meta, cancellationToken);

        // 保存 AI 回复
        var assistantMessage = new AssistantMessage
        {
            SessionId = session.Id,
            Role = "Assistant",
            Content = aiResponse,
            CreatedAt = DateTime.Now
        };
        DbContext.AssistantMessages.Add(assistantMessage);
        session.UpdatedAt = DateTime.Now;
        await DbContext.SaveChangesAsync(cancellationToken);

        // 解析响应，提取建议
        var suggestions = ExtractSuggestions(aiResponse);

        return new AssistantChatResult
        {
            Success = true,
            Reply = aiResponse,
            SessionId = session.Id,
            Suggestions = suggestions
        };
    }

    /// <summary>
    /// 生成会话标题
    /// </summary>
    private string GenerateSessionTitle(string firstMessage)
    {
        if (string.IsNullOrEmpty(firstMessage))
        {
            return $"会话 {DateTime.Now:yyyy-MM-dd HH:mm}";
        }

        // 取前30个字符作为标题
        var title = firstMessage.Length > 30
            ? firstMessage.Substring(0, 30) + "..."
            : firstMessage;

        return title;
    }

    /// <summary>
    /// 构建 Prompt
    /// </summary>
    private string BuildPrompt(AssistantChatRequest request, List<AssistantMessage> historyMessages)
    {
        var sb = new StringBuilder();

        // 系统提示词
        sb.AppendLine("你是溪午听风个人网站管理员的专属 AI 助理。你的任务是帮助管理员分析线索、规划学习、安排项目优先级、分析数据等。");
        sb.AppendLine("请提供专业、实用的建议，保持简洁明了的回答风格。");
        sb.AppendLine();

        // 根据上下文类型添加额外信息
        switch (request.ContextType?.ToLower())
        {
            case "leads":
                sb.AppendLine(GetLeadsContext());
                break;
            case "projects":
                sb.AppendLine(GetProjectsContext());
                break;
            case "stats":
                sb.AppendLine(GetStatsContext());
                break;
        }

        // 添加历史对话
        if (historyMessages.Any())
        {
            sb.AppendLine();
            sb.AppendLine("以下是之前的对话历史：");
            foreach (var msg in historyMessages.TakeLast(10)) // 只取最近10条
            {
                sb.AppendLine($"{msg.Role}: {msg.Content}");
            }
            sb.AppendLine();
        }

        // 当前用户消息
        sb.AppendLine($"User: {request.Message}");
        sb.AppendLine();
        sb.AppendLine("请以 JSON 格式返回，包含以下字段：");
        sb.AppendLine("- reply: 回答内容（Markdown 格式）");
        sb.AppendLine("- suggestions: 建议列表（数组，3-5个快捷问题或操作建议）");

        return sb.ToString();
    }

    /// <summary>
    /// 获取线索上下文
    /// </summary>
    private string GetLeadsContext()
    {
        try
        {
            var recentLeads = DbContext.PreSaleConsultations
                .OrderByDescending(l => l.CreatedAt)
                .Take(5)
                .Select(l => new
                {
                    l.Id,
                    l.CustomerName,
                    l.RequirementDescription,
                    l.Status,
                    l.Score
                })
                .ToList();

            if (recentLeads.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine("最近的线索信息：");
                foreach (var lead in recentLeads)
                {
                    var description = lead.RequirementDescription ?? "";
                    var shortDescription = description.Length > 50 
                        ? description.Substring(0, 50) + "..." 
                        : description;
                    sb.AppendLine($"- 线索 #{lead.Id}: {lead.CustomerName}, 需求: {shortDescription}, 状态: {lead.Status}, 评分: {lead.Score ?? 0}");
                }
                return sb.ToString();
            }
        }
        catch
        {
            // 忽略错误
        }

        return "";
    }

    /// <summary>
    /// 获取项目上下文
    /// </summary>
    private string GetProjectsContext()
    {
        try
        {
            var recentProjects = DbContext.Projects
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .Select(p => new { p.Id, p.Title, p.Status, p.ViewCount })
                .ToList();

            if (recentProjects.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine("最近的项目：");
                foreach (var project in recentProjects)
                {
                    sb.AppendLine($"- {project.Title} (状态: {project.Status}, 访问量: {project.ViewCount})");
                }
                return sb.ToString();
            }
        }
        catch
        {
            // 忽略错误
        }

        return "";
    }

    /// <summary>
    /// 获取统计数据上下文
    /// </summary>
    private string GetStatsContext()
    {
        try
        {
            var articleCount = DbContext.Articles.Count(a => a.Status == 1);
            var projectCount = DbContext.Projects.Count();
            var consultationCount = DbContext.PreSaleConsultations.Count(c => c.Status == 0);

            return $"当前数据统计：文章 {articleCount} 篇，项目 {projectCount} 个，待处理咨询 {consultationCount} 条";
        }
        catch
        {
            return "";
        }
    }

    /// <summary>
    /// 提取建议
    /// </summary>
    private List<string> ExtractSuggestions(string aiResponse)
    {
        try
        {
            var jsonStart = aiResponse.IndexOf('{');
            var jsonEnd = aiResponse.LastIndexOf('}');
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonStr = aiResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
                var jsonDoc = JsonDocument.Parse(jsonStr);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("suggestions", out var suggestionsEl) && suggestionsEl.ValueKind == JsonValueKind.Array)
                {
                    var suggestions = new List<string>();
                    foreach (var item in suggestionsEl.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                        {
                            suggestions.Add(item.GetString() ?? "");
                        }
                    }
                    return suggestions;
                }
            }
        }
        catch
        {
            // 忽略错误
        }

        // 默认建议
        return new List<string>
        {
            "帮我看下最近的线索情况",
            "帮我分析最近哪些项目优先做",
            "帮我生成一周学习计划"
        };
    }
}

/// <summary>
/// 个人助理对话请求 DTO
/// </summary>
public class AssistantChatRequest
{
    /// <summary>
    /// 会话 ID（可选，如果为空则创建新会话）
    /// </summary>
    public long? SessionId { get; set; }

    /// <summary>
    /// 用户输入消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 上下文类型（general / leads / projects / stats）
    /// </summary>
    public string? ContextType { get; set; }

    /// <summary>
    /// 用户 ID（从当前登录用户上下文获取）
    /// </summary>
    public long UserId { get; set; }
}

/// <summary>
/// 个人助理对话响应 DTO
/// </summary>
public class AssistantChatResult
{
    public bool Success { get; set; }
    public string Reply { get; set; } = string.Empty;
    public long SessionId { get; set; }
    public List<string> Suggestions { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

