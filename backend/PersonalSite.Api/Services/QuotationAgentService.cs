using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Services;

/// <summary>
/// 自动报价智能体服务
/// 根据客户需求和项目信息，自动生成报价方案
/// </summary>
public class QuotationAgentService : AiAgentService
{
    public QuotationAgentService(
        AiServiceClient aiClient,
        AppDbContext dbContext,
        ILogger<QuotationAgentService> logger)
        : base(aiClient, dbContext, logger)
    {
    }

    /// <summary>
    /// 生成报价方案
    /// </summary>
    public async Task<QuotationResult> GenerateQuotationAsync(
        QuotationRequest request,
        CancellationToken cancellationToken = default)
    {
        // 读取线索信息
        var consultation = await DbContext.PreSaleConsultations
            .FirstOrDefaultAsync(c => c.Id == request.LeadId, cancellationToken);

        if (consultation == null)
        {
            throw new Exception($"咨询记录不存在: {request.LeadId}");
        }

        // 读取相关项目/工具信息（如果有）
        var relatedProject = await DbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

        // 构建 Prompt
        var prompt = BuildPrompt(request, consultation, relatedProject);

        // 调用 AI
        var meta = new Dictionary<string, object>
        {
            ["LeadId"] = request.LeadId.ToString(),
            ["ProjectId"] = request.ProjectId?.ToString() ?? "",
            ["BudgetRange"] = consultation.BudgetRange ?? ""
        };

        string aiResponse;
        try
        {
            aiResponse = await CallAiAsync("Quotation", prompt, meta, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "调用 AI 服务失败: LeadId={LeadId}", request.LeadId);
            return new QuotationResult
            {
                Success = false,
                ErrorMessage = $"AI 服务调用失败: {ex.Message}。请检查 AI 服务是否正常运行。"
            };
        }

        // 解析响应
        var result = ParseAiResponse(aiResponse, request);

        return result;
    }

    /// <summary>
    /// 构建 Prompt
    /// </summary>
    private string BuildPrompt(QuotationRequest request, PreSaleConsultation consultation, Project? project)
    {
        var sb = new StringBuilder();
        sb.AppendLine("请根据以下客户需求和项目信息，生成一份专业的报价方案：");
        sb.AppendLine();

        sb.AppendLine("客户信息：");
        sb.AppendLine($"- 姓名：{consultation.CustomerName}");
        if (!string.IsNullOrEmpty(consultation.CustomerPhone))
        {
            sb.AppendLine($"- 电话：{consultation.CustomerPhone}");
        }
        if (!string.IsNullOrEmpty(consultation.BudgetRange))
        {
            sb.AppendLine($"- 预算范围：{consultation.BudgetRange}");
        }
        if (!string.IsNullOrEmpty(consultation.ExpectedDeadline))
        {
            sb.AppendLine($"- 期望完成时间：{consultation.ExpectedDeadline}");
        }

        sb.AppendLine();
        sb.AppendLine("需求描述：");
        sb.AppendLine(consultation.RequirementDescription);

        if (project != null)
        {
            sb.AppendLine();
            sb.AppendLine("相关项目信息：");
            sb.AppendLine($"- 项目名称：{project.Title}");
            sb.AppendLine($"- 项目描述：{project.Description}");
            if (!string.IsNullOrEmpty(project.TechStack))
            {
                sb.AppendLine($"- 技术栈：{project.TechStack}");
            }
        }

        if (!string.IsNullOrEmpty(request.ExtraNotes))
        {
            sb.AppendLine();
            sb.AppendLine("额外说明：");
            sb.AppendLine(request.ExtraNotes);
        }

        sb.AppendLine();
        sb.AppendLine("请以 JSON 格式返回，包含以下字段：");
        sb.AppendLine("- title: 报价方案标题");
        sb.AppendLine("- summary: 方案摘要（100-200字）");
        sb.AppendLine("- items: 报价明细列表（数组，每个项目包含：name-项目名称, description-描述, quantity-数量, unitPrice-单价, total-小计）");
        sb.AppendLine("- totalAmount: 总金额");
        sb.AppendLine("- paymentTerms: 付款方式（如：首付30%，验收后付70%）");
        sb.AppendLine("- deliveryTime: 交付时间");
        sb.AppendLine("- warranty: 质保说明");
        sb.AppendLine("- notes: 备注说明");

        return sb.ToString();
    }

    /// <summary>
    /// 解析 AI 响应
    /// </summary>
    private QuotationResult ParseAiResponse(string aiResponse, QuotationRequest request)
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

                var items = new List<QuotationItem>();
                if (root.TryGetProperty("items", out var itemsEl) && itemsEl.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in itemsEl.EnumerateArray())
                    {
                        items.Add(new QuotationItem
                        {
                            Name = item.TryGetProperty("name", out var name) ? name.GetString() ?? "" : "",
                            Description = item.TryGetProperty("description", out var desc) ? desc.GetString() ?? "" : "",
                            Quantity = item.TryGetProperty("quantity", out var qty) && qty.ValueKind == JsonValueKind.Number ? qty.GetInt32() : 1,
                            UnitPrice = item.TryGetProperty("unitPrice", out var price) && price.ValueKind == JsonValueKind.Number ? price.GetDecimal() : 0,
                            Total = item.TryGetProperty("total", out var total) && total.ValueKind == JsonValueKind.Number ? total.GetDecimal() : 0
                        });
                    }
                }

                return new QuotationResult
                {
                    Success = true,
                    Quotation = new QuotationDto
                    {
                        Title = root.TryGetProperty("title", out var title) ? title.GetString() ?? "" : "",
                        Summary = root.TryGetProperty("summary", out var summary) ? summary.GetString() ?? "" : "",
                        Items = items,
                        TotalAmount = root.TryGetProperty("totalAmount", out var totalAmount) && totalAmount.ValueKind == JsonValueKind.Number ? totalAmount.GetDecimal() : 0,
                        PaymentTerms = root.TryGetProperty("paymentTerms", out var paymentTerms) ? paymentTerms.GetString() ?? "" : "",
                        DeliveryTime = root.TryGetProperty("deliveryTime", out var deliveryTime) ? deliveryTime.GetString() ?? "" : "",
                        Warranty = root.TryGetProperty("warranty", out var warranty) ? warranty.GetString() ?? "" : "",
                        Notes = root.TryGetProperty("notes", out var notes) ? notes.GetString() ?? "" : ""
                    }
                };
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "解析 AI 响应 JSON 失败，使用默认值");
        }

        // 如果解析失败，返回默认值
        return new QuotationResult
        {
            Success = true,
            Quotation = new QuotationDto
            {
                Title = "报价方案",
                Summary = "",
                Items = new List<QuotationItem>(),
                TotalAmount = 0,
                PaymentTerms = "",
                DeliveryTime = "",
                Warranty = "",
                Notes = ""
            }
        };
    }
}

/// <summary>
/// 报价请求 DTO
/// </summary>
public class QuotationRequest
{
    /// <summary>
    /// 线索 ID（PreSaleConsultation.Id）
    /// </summary>
    public long LeadId { get; set; }

    /// <summary>
    /// 项目 ID（可选，如果有关联项目）
    /// </summary>
    public Guid? ProjectId { get; set; }

    /// <summary>
    /// 额外说明
    /// </summary>
    public string? ExtraNotes { get; set; }
}

/// <summary>
/// 报价响应 DTO
/// </summary>
public class QuotationResult
{
    public bool Success { get; set; }
    public QuotationDto? Quotation { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// 报价方案 DTO
/// </summary>
public class QuotationDto
{
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<QuotationItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string PaymentTerms { get; set; } = string.Empty;
    public string DeliveryTime { get; set; } = string.Empty;
    public string Warranty { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// 报价明细项
/// </summary>
public class QuotationItem
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}

