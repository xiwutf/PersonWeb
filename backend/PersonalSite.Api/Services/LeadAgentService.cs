using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Services;

/// <summary>
/// 表单线索处理智能体服务
/// 用于分析用户提交的咨询/需求，生成摘要、标签、评分和推荐建议
/// </summary>
public class LeadAgentService : AiAgentService
{
    public LeadAgentService(
        AiServiceClient aiClient,
        AppDbContext dbContext,
        ILogger<LeadAgentService> logger)
        : base(aiClient, dbContext, logger)
    {
    }

    /// <summary>
    /// 分析线索
    /// </summary>
    public async Task<LeadAnalysisResult> AnalyzeLeadAsync(
        LeadAnalysisRequest request,
        CancellationToken cancellationToken = default)
    {
        // 读取线索数据
        var consultation = await DbContext.PreSaleConsultations
            .FirstOrDefaultAsync(c => c.Id == request.LeadId, cancellationToken);

        if (consultation == null)
        {
            throw new Exception($"咨询记录不存在: {request.LeadId}");
        }

        // 构建 Prompt
        var prompt = BuildPrompt(request, consultation);

        // 调用 AI
        var meta = new Dictionary<string, object>
        {
            ["LeadId"] = request.LeadId.ToString(),
            ["ProductId"] = consultation.ProductId > 0 ? consultation.ProductId.ToString() : "",
            ["CustomerName"] = consultation.CustomerName ?? ""
        };

        string aiResponse;
        try
        {
            aiResponse = await CallAiAsync("Lead", prompt, meta, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "调用 AI 服务失败: LeadId={LeadId}", request.LeadId);
            return new LeadAnalysisResult
            {
                Success = false,
                ErrorMessage = $"AI 服务调用失败: {ex.Message}。请检查 AI 服务是否正常运行。"
            };
        }

        // 解析响应
        var analysis = ParseAiResponse(aiResponse);

        // 写回数据库
        try
        {
            consultation.Summary = analysis.Summary;
            consultation.Tags = JsonSerializer.Serialize(analysis.Tags);
            consultation.Score = analysis.Score;
            consultation.AiRecommendation = analysis.Recommendation;
            consultation.UpdatedAt = DateTime.Now;

            await DbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "保存分析结果到数据库失败: LeadId={LeadId}", request.LeadId);
            return new LeadAnalysisResult
            {
                Success = false,
                ErrorMessage = $"保存分析结果失败: {ex.Message}。请检查数据库字段是否存在。"
            };
        }

        return new LeadAnalysisResult
        {
            Success = true,
            Analysis = analysis
        };
    }

    /// <summary>
    /// 构建 Prompt
    /// </summary>
    private string BuildPrompt(LeadAnalysisRequest request, PreSaleConsultation consultation)
    {
        var sb = new StringBuilder();
        sb.AppendLine("请分析以下客户咨询/需求，生成摘要、标签、评分和推荐建议：");
        sb.AppendLine();

        sb.AppendLine($"客户姓名：{consultation.CustomerName}");
        if (!string.IsNullOrEmpty(consultation.CustomerPhone))
        {
            sb.AppendLine($"联系电话：{consultation.CustomerPhone}");
        }
        if (!string.IsNullOrEmpty(consultation.CustomerEmail))
        {
            sb.AppendLine($"邮箱：{consultation.CustomerEmail}");
        }
        if (!string.IsNullOrEmpty(consultation.BudgetRange))
        {
            sb.AppendLine($"预算范围：{consultation.BudgetRange}");
        }
        if (!string.IsNullOrEmpty(consultation.ExpectedDeadline))
        {
            sb.AppendLine($"期望完成时间：{consultation.ExpectedDeadline}");
        }

        sb.AppendLine();
        sb.AppendLine("需求描述：");
        sb.AppendLine(consultation.RequirementDescription);

        if (!string.IsNullOrEmpty(request.RawText))
        {
            sb.AppendLine();
            sb.AppendLine("补充信息：");
            sb.AppendLine(request.RawText);
        }

        if (request.Meta != null && request.Meta.Any())
        {
            sb.AppendLine();
            sb.AppendLine("其他信息：");
            foreach (var kvp in request.Meta)
            {
                sb.AppendLine($"{kvp.Key}: {kvp.Value}");
            }
        }

        sb.AppendLine();
        sb.AppendLine("请以 JSON 格式返回，包含以下字段：");
        sb.AppendLine("- summary: 需求摘要（100-200字）");
        sb.AppendLine("- tags: 标签列表（数组，3-5个标签，例如：[\"紧急\", \"高预算\", \"定制开发\"]）");
        sb.AppendLine("- score: 线索质量评分（0-100，综合考虑预算、需求明确度、时间紧迫性等）");
        sb.AppendLine("- recommendation: 推荐建议（如何处理这个线索，例如：\"高价值线索，建议优先联系\"）");

        return sb.ToString();
    }

    /// <summary>
    /// 解析 AI 响应
    /// </summary>
    private LeadAnalysisDto ParseAiResponse(string aiResponse)
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

                var tags = new List<string>();
                if (root.TryGetProperty("tags", out var tagsEl) && tagsEl.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in tagsEl.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                        {
                            tags.Add(item.GetString() ?? "");
                        }
                    }
                }

                return new LeadAnalysisDto
                {
                    Summary = root.TryGetProperty("summary", out var summary) ? summary.GetString() ?? "" : "",
                    Tags = tags,
                    Score = root.TryGetProperty("score", out var score) && score.ValueKind == JsonValueKind.Number
                        ? score.GetInt32()
                        : null,
                    Recommendation = root.TryGetProperty("recommendation", out var recommendation)
                        ? recommendation.GetString() ?? ""
                        : ""
                };
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "解析 AI 响应 JSON 失败，使用默认值");
        }

        // 如果解析失败，返回默认值
        return new LeadAnalysisDto
        {
            Summary = "",
            Tags = new List<string>(),
            Score = null,
            Recommendation = ""
        };
    }
}

/// <summary>
/// 线索分析请求 DTO
/// </summary>
public class LeadAnalysisRequest
{
    /// <summary>
    /// 线索 ID（PreSaleConsultation.Id）
    /// </summary>
    public long LeadId { get; set; }

    /// <summary>
    /// 用户原始需求描述（可选，如果为空则使用数据库中的记录）
    /// </summary>
    public string? RawText { get; set; }

    /// <summary>
    /// 元数据（预算、截止日期、渠道、联系方式等）
    /// </summary>
    public Dictionary<string, object>? Meta { get; set; }
}

/// <summary>
/// 线索分析响应 DTO
/// </summary>
public class LeadAnalysisResult
{
    public bool Success { get; set; }
    public LeadAnalysisDto? Analysis { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// 线索分析结果 DTO
/// </summary>
public class LeadAnalysisDto
{
    public string Summary { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public int? Score { get; set; }
    public string Recommendation { get; set; } = string.Empty;
}

