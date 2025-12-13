using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Text.Json;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PriceAlertController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public PriceAlertController(AppDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// 获取价格提醒列表
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetList()
    {
        var alerts = await _context.PriceAlerts
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new
            {
                a.Id,
                a.Code,
                a.Name,
                a.Type,
                a.TargetPrice,
                a.AlertType,
                a.CurrentPrice,
                a.IsTriggered,
                a.TriggeredAt,
                a.IsActive,
                a.NotificationSent,
                a.Notes,
                a.CreatedAt,
                a.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(alerts));
    }

    /// <summary>
    /// 获取价格提醒详情
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetById(long id)
    {
        var alert = await _context.PriceAlerts.FindAsync(id);
        if (alert == null)
        {
            return Ok(ApiResponse.Error("未找到", 404));
        }

        var result = new
        {
            alert.Id,
            alert.Code,
            alert.Name,
            alert.Type,
            alert.TargetPrice,
            alert.AlertType,
            alert.CurrentPrice,
            alert.IsTriggered,
            alert.TriggeredAt,
            alert.IsActive,
            alert.NotificationSent,
            alert.Notes,
            alert.CreatedAt,
            alert.UpdatedAt
        };

        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// 创建价格提醒
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Create([FromBody] PriceAlertRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Code) || string.IsNullOrEmpty(request.Type))
            {
                return Ok(ApiResponse.Error("代码和类型不能为空", 400));
            }

            if (request.TargetPrice <= 0)
            {
                return Ok(ApiResponse.Error("目标价格必须大于0", 400));
            }

            // 获取名称和当前价格
            string name = request.Name ?? string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                name = await GetNameFromCode(request.Code, request.Type);
            }

            decimal currentPrice = await GetCurrentPrice(request.Code, request.Type);

            var alert = new PriceAlert
            {
                Code = request.Code,
                Name = name,
                Type = request.Type,
                TargetPrice = request.TargetPrice,
                AlertType = request.AlertType ?? "above",
                CurrentPrice = currentPrice,
                IsActive = request.IsActive ?? true,
                Notes = request.Notes,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.PriceAlerts.Add(alert);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new { id = alert.Id }, "创建成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"创建失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 更新价格提醒
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Update(long id, [FromBody] PriceAlertRequest request)
    {
        try
        {
            var alert = await _context.PriceAlerts.FindAsync(id);
            if (alert == null)
            {
                return Ok(ApiResponse.Error("未找到", 404));
            }

            if (!string.IsNullOrEmpty(request.Code))
            {
                alert.Code = request.Code;
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                alert.Name = request.Name;
            }

            if (!string.IsNullOrEmpty(request.Type))
            {
                alert.Type = request.Type;
            }

            if (request.TargetPrice > 0)
            {
                alert.TargetPrice = request.TargetPrice;
            }

            if (!string.IsNullOrEmpty(request.AlertType))
            {
                alert.AlertType = request.AlertType;
            }

            if (request.IsActive.HasValue)
            {
                alert.IsActive = request.IsActive.Value;
            }

            if (request.Notes != null)
            {
                alert.Notes = request.Notes;
            }

            alert.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(null, "更新成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"更新失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 删除价格提醒
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Delete(long id)
    {
        try
        {
            var alert = await _context.PriceAlerts.FindAsync(id);
            if (alert == null)
            {
                return Ok(ApiResponse.Error("未找到", 404));
            }

            _context.PriceAlerts.Remove(alert);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(null, "删除成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"删除失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 刷新所有价格提醒的当前价格
    /// </summary>
    [HttpPost("refresh-prices")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> RefreshPrices()
    {
        try
        {
            var alerts = await _context.PriceAlerts
                .Where(a => a.IsActive && !a.IsTriggered)
                .ToListAsync();

            int updatedCount = 0;
            int triggeredCount = 0;

            foreach (var alert in alerts)
            {
                decimal currentPrice = await GetCurrentPrice(alert.Code, alert.Type);
                if (currentPrice > 0)
                {
                    alert.CurrentPrice = currentPrice;

                    // 检查是否触发
                    bool shouldTrigger = false;
                    if (alert.AlertType == "above" && currentPrice >= alert.TargetPrice)
                    {
                        shouldTrigger = true;
                    }
                    else if (alert.AlertType == "below" && currentPrice <= alert.TargetPrice)
                    {
                        shouldTrigger = true;
                    }
                    else if (alert.AlertType == "both")
                    {
                        // 检查是否从上方或下方穿过目标价格
                        // 这里简化处理，实际应该记录历史价格来判断
                        if (Math.Abs(currentPrice - alert.TargetPrice) < alert.TargetPrice * 0.01m) // 1% 范围内
                        {
                            shouldTrigger = true;
                        }
                    }

                    if (shouldTrigger && !alert.IsTriggered)
                    {
                        alert.IsTriggered = true;
                        alert.TriggeredAt = DateTime.Now;
                        triggeredCount++;
                    }

                    alert.UpdatedAt = DateTime.Now;
                    updatedCount++;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new
            {
                updatedCount,
                triggeredCount
            }, $"刷新成功，更新 {updatedCount} 条，触发 {triggeredCount} 条"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"刷新失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取当前价格（复用 InvestmentController 的逻辑）
    /// </summary>
    private async Task<decimal> GetCurrentPrice(string? code, string? type)
    {
        if (string.IsNullOrEmpty(code))
        {
            return 0;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            var paddedCode = code.PadLeft(6, '0');

            // 判断是否为场外基金
            bool isOTCFund = type?.ToLower() == "fund" &&
                            (paddedCode.StartsWith("00") || paddedCode.StartsWith("01") || paddedCode.StartsWith("05"));

            if (isOTCFund)
            {
                return 0; // 暂时返回0，需要实现场外基金价格获取
            }

            // 构建 secid 参数
            string secid;
            if (type?.ToLower() == "fund")
            {
                if (paddedCode.StartsWith("51") || paddedCode.StartsWith("52") ||
                    paddedCode.StartsWith("53") || paddedCode.StartsWith("54") ||
                    paddedCode.StartsWith("55") || paddedCode.StartsWith("56") ||
                    paddedCode.StartsWith("57") || paddedCode.StartsWith("58") ||
                    paddedCode.StartsWith("59"))
                {
                    secid = $"1.{paddedCode}";
                }
                else if (paddedCode.StartsWith("15") || paddedCode.StartsWith("16"))
                {
                    secid = $"0.{paddedCode}";
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                secid = $"1.{paddedCode}";
            }

            var url = $"https://push2.eastmoney.com/api/qt/stock/get?secid={secid}&fields=f43,f57,f58";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                if (result.TryGetProperty("rc", out var rcElement))
                {
                    var rc = rcElement.GetInt32();
                    if (rc != 0)
                    {
                        return 0;
                    }
                }

                if (result.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object)
                {
                    if (data.TryGetProperty("f43", out var priceElement) && priceElement.ValueKind != JsonValueKind.Null)
                    {
                        var priceStr = priceElement.GetRawText();
                        if (decimal.TryParse(priceStr, out var price) && price > 0)
                        {
                            return price / 100; // 价格单位是分，需要除以100
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取价格失败 {code}: {ex.Message}");
        }

        return 0;
    }

    /// <summary>
    /// 根据代码获取名称
    /// </summary>
    private async Task<string> GetNameFromCode(string? code, string? type)
    {
        // 这里可以调用 InvestmentController 的 GetNameFromCode 方法
        // 或者直接实现相同的逻辑
        return string.Empty;
    }
}

/// <summary>
/// 价格提醒请求模型
/// </summary>
public class PriceAlertRequest
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public decimal TargetPrice { get; set; }
    public string? AlertType { get; set; } // above / below / both
    public bool? IsActive { get; set; }
    public string? Notes { get; set; }
}
