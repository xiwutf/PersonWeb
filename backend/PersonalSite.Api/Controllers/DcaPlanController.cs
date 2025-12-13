using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Text.Json;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DcaPlanController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public DcaPlanController(AppDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// 获取定投计划列表
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetList()
    {
        var plans = await _context.DcaPlans
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.Id,
                p.Code,
                p.Name,
                p.Type,
                p.Amount,
                p.Frequency,
                p.NextExecutionDate,
                p.LastExecutionDate,
                p.TotalExecutions,
                p.TotalInvested,
                p.IsActive,
                p.StartDate,
                p.EndDate,
                p.Notes,
                p.CreatedAt,
                p.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(plans));
    }

    /// <summary>
    /// 获取定投计划详情
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetById(long id)
    {
        var plan = await _context.DcaPlans
            .Include(p => p.Executions)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plan == null)
        {
            return Ok(ApiResponse.Error("未找到", 404));
        }

        var result = new
        {
            plan.Id,
            plan.Code,
            plan.Name,
            plan.Type,
            plan.Amount,
            plan.Frequency,
            plan.NextExecutionDate,
            plan.LastExecutionDate,
            plan.TotalExecutions,
            plan.TotalInvested,
            plan.IsActive,
            plan.StartDate,
            plan.EndDate,
            plan.Notes,
            plan.CreatedAt,
            plan.UpdatedAt,
            Executions = plan.Executions.Select(e => new
            {
                e.Id,
                e.ExecutionDate,
                e.Amount,
                e.Price,
                e.Quantity,
                e.Status,
                e.ErrorMessage,
                e.Notes,
                e.CreatedAt
            }).ToList()
        };

        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// 创建定投计划
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Create([FromBody] DcaPlanRequest request)
    {
        try
        {
            // 验证必填字段
            if (string.IsNullOrEmpty(request.Code) || string.IsNullOrEmpty(request.Type))
            {
                return Ok(ApiResponse.Error("代码和类型不能为空", 400));
            }

            if (request.Amount <= 0)
            {
                return Ok(ApiResponse.Error("定投金额必须大于0", 400));
            }

            // 获取名称（如果未提供）
            string name = request.Name ?? string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                name = await GetNameFromCode(request.Code, request.Type);
            }

            // 计算下次执行日期
            DateTime? nextExecutionDate = CalculateNextExecutionDate(
                request.Frequency ?? "monthly",
                request.StartDate ?? DateTime.Now
            );

            var plan = new DcaPlan
            {
                Code = request.Code,
                Name = name,
                Type = request.Type,
                Amount = request.Amount,
                Frequency = request.Frequency ?? "monthly",
                NextExecutionDate = nextExecutionDate,
                IsActive = request.IsActive ?? true,
                StartDate = request.StartDate ?? DateTime.Now,
                EndDate = request.EndDate,
                Notes = request.Notes,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.DcaPlans.Add(plan);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new { id = plan.Id }, "创建成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"创建失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 更新定投计划
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Update(long id, [FromBody] DcaPlanRequest request)
    {
        try
        {
            var plan = await _context.DcaPlans.FindAsync(id);
            if (plan == null)
            {
                return Ok(ApiResponse.Error("未找到", 404));
            }

            // 更新字段
            if (!string.IsNullOrEmpty(request.Code))
            {
                plan.Code = request.Code;
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                plan.Name = request.Name;
            }

            if (!string.IsNullOrEmpty(request.Type))
            {
                plan.Type = request.Type;
            }

            if (request.Amount > 0)
            {
                plan.Amount = request.Amount;
            }

            if (!string.IsNullOrEmpty(request.Frequency))
            {
                plan.Frequency = request.Frequency;
                // 重新计算下次执行日期
                plan.NextExecutionDate = CalculateNextExecutionDate(
                    request.Frequency,
                    plan.StartDate
                );
            }

            if (request.IsActive.HasValue)
            {
                plan.IsActive = request.IsActive.Value;
            }

            if (request.StartDate.HasValue)
            {
                plan.StartDate = request.StartDate.Value;
            }

            if (request.EndDate.HasValue)
            {
                plan.EndDate = request.EndDate;
            }

            if (request.Notes != null)
            {
                plan.Notes = request.Notes;
            }

            plan.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(null, "更新成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"更新失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 删除定投计划
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Delete(long id)
    {
        try
        {
            var plan = await _context.DcaPlans.FindAsync(id);
            if (plan == null)
            {
                return Ok(ApiResponse.Error("未找到", 404));
            }

            // 删除关联的执行记录
            var executions = await _context.DcaExecutions
                .Where(e => e.DcaPlanId == id)
                .ToListAsync();
            _context.DcaExecutions.RemoveRange(executions);

            // 删除定投计划
            _context.DcaPlans.Remove(plan);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(null, "删除成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"删除失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 手动执行定投
    /// </summary>
    [HttpPost("{id}/execute")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Execute(long id)
    {
        try
        {
            var plan = await _context.DcaPlans.FindAsync(id);
            if (plan == null)
            {
                return Ok(ApiResponse.Error("未找到定投计划", 404));
            }

            if (!plan.IsActive)
            {
                return Ok(ApiResponse.Error("定投计划未启用", 400));
            }

            // 检查是否已过期
            if (plan.EndDate.HasValue && plan.EndDate.Value < DateTime.Now)
            {
                return Ok(ApiResponse.Error("定投计划已过期", 400));
            }

            // 获取当前价格
            decimal currentPrice = await GetCurrentPrice(plan.Code, plan.Type);
            if (currentPrice <= 0)
            {
                return Ok(ApiResponse.Error("无法获取当前价格", 400));
            }

            // 计算买入数量
            decimal quantity = plan.Amount / currentPrice;

            // 创建执行记录
            var execution = new DcaExecution
            {
                DcaPlanId = plan.Id,
                ExecutionDate = DateTime.Now,
                Amount = plan.Amount,
                Price = currentPrice,
                Quantity = quantity,
                Status = "completed",
                CreatedAt = DateTime.Now
            };

            _context.DcaExecutions.Add(execution);

            // 更新定投计划
            plan.LastExecutionDate = DateTime.Now;
            plan.TotalExecutions += 1;
            plan.TotalInvested += plan.Amount;
            plan.NextExecutionDate = CalculateNextExecutionDate(plan.Frequency, DateTime.Now);
            plan.UpdatedAt = DateTime.Now;

            // 更新或创建投资记录
            await UpdateOrCreateInvestment(plan.Code, plan.Name, plan.Type, quantity, currentPrice, plan.Amount);

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new
            {
                executionId = execution.Id,
                price = currentPrice,
                quantity = quantity,
                amount = plan.Amount
            }, "执行成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"执行失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取执行记录列表
    /// </summary>
    [HttpGet("{id}/executions")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetExecutions(long id)
    {
        var executions = await _context.DcaExecutions
            .Where(e => e.DcaPlanId == id)
            .OrderByDescending(e => e.ExecutionDate)
            .Select(e => new
            {
                e.Id,
                e.ExecutionDate,
                e.Amount,
                e.Price,
                e.Quantity,
                e.Status,
                e.ErrorMessage,
                e.Notes,
                e.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(executions));
    }

    /// <summary>
    /// 计算下次执行日期
    /// </summary>
    private DateTime? CalculateNextExecutionDate(string frequency, DateTime startDate)
    {
        return frequency.ToLower() switch
        {
            "daily" => startDate.AddDays(1),
            "weekly" => startDate.AddDays(7),
            "monthly" => startDate.AddMonths(1),
            "quarterly" => startDate.AddMonths(3),
            _ => startDate.AddMonths(1)
        };
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
                // 场外基金：从基金详情页面获取最新净值
                // 这里可以调用 InvestmentController 的方法，或者直接实现
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

    /// <summary>
    /// 更新或创建投资记录
    /// </summary>
    private async Task UpdateOrCreateInvestment(string code, string name, string type, decimal quantity, decimal price, decimal amount)
    {
        var investment = await _context.Investments
            .FirstOrDefaultAsync(i => i.Code == code);

        if (investment == null)
        {
            // 创建新的投资记录
            investment = new Investment
            {
                Code = code,
                Name = name,
                Type = type,
                Quantity = quantity,
                CostPrice = price,
                CurrentPrice = price,
                TotalCost = amount,
                MarketValue = quantity * price,
                ProfitLoss = 0,
                ProfitRate = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Investments.Add(investment);
        }
        else
        {
            // 更新现有投资记录（计算平均成本价）
            var totalCost = investment.TotalCost + amount;
            var totalQuantity = investment.Quantity + quantity;
            investment.Quantity = totalQuantity;
            investment.CostPrice = totalQuantity > 0 ? totalCost / totalQuantity : 0;
            investment.TotalCost = totalCost;
            investment.CurrentPrice = price;
            investment.MarketValue = totalQuantity * price;
            investment.ProfitLoss = investment.MarketValue - investment.TotalCost;
            investment.ProfitRate = investment.TotalCost > 0
                ? (investment.ProfitLoss / investment.TotalCost) * 100
                : 0;
            investment.UpdatedAt = DateTime.Now;
        }

        // 创建交易记录
        var transaction = new InvestmentTransaction
        {
            InvestmentId = investment.Id,
            TransactionType = "buy",
            Quantity = quantity,
            Price = price,
            Amount = amount,
            Fee = 0,
            TransactionDate = DateTime.Now,
            Notes = "定投自动买入",
            CreatedAt = DateTime.Now
        };
        _context.InvestmentTransactions.Add(transaction);
    }
}

/// <summary>
/// 定投计划请求模型
/// </summary>
public class DcaPlanRequest
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public decimal Amount { get; set; }
    public string? Frequency { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }
}
