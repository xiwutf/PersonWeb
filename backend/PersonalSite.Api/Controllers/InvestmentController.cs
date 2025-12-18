using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Text.Json;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvestmentController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public InvestmentController(AppDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// 获取投资列表
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetList()
    {
        var investments = await _context.Investments
            .OrderByDescending(i => i.UpdatedAt)
            .Select(i => new
            {
                i.Id,
                i.Code,
                i.Name,
                i.Type,
                i.Quantity,
                i.CostPrice,
                i.CurrentPrice,
                i.TotalCost,
                i.MarketValue,
                i.ProfitLoss,
                i.ProfitRate,
                i.Notes,
                i.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(investments));
    }

    /// <summary>
    /// 根据代码自动获取名称和价格（用于表单自动填充）
    /// </summary>
    [HttpGet("auto-fill")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> AutoFill([FromQuery] string? code, [FromQuery] string? type)
    {
        try
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(type))
            {
                return BadRequest(ApiResponse.Error("代码和类型不能为空", 400));
            }

            var price = await GetCurrentPrice(code, type);
            var name = await GetNameFromCode(code, type);

            return Ok(ApiResponse.Success(new
            {
                Name = name ?? string.Empty,
                CurrentPrice = price
            }));
        }
        catch (Exception ex)
        {
            // 只记录错误日志
            Console.Error.WriteLine($"[AutoFill] 异常: {ex.Message}");
            return StatusCode(500, ApiResponse.Error($"获取失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取投资详情
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetDetail(long id)
    {
        var investment = await _context.Investments
            .Include(i => i.Transactions)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (investment == null)
        {
            return Ok(ApiResponse.Error("未找到", 404));
        }

        return Ok(ApiResponse.Success(investment));
    }

    /// <summary>
    /// 创建投资记录
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Create([FromBody] InvestmentRequest request)
    {
        try
        {
            // 获取实时价格
            if (string.IsNullOrEmpty(request.Code) || string.IsNullOrEmpty(request.Type))
            {
                return Ok(ApiResponse.Error("代码和类型不能为空", 400));
            }

            // 检查是否已存在相同代码的投资记录
            var existingInvestment = await _context.Investments
                .FirstOrDefaultAsync(i => i.Code == request.Code);
            
            if (existingInvestment != null)
            {
                // 如果代码已存在，自动转换为"添加买入交易"
                // 这样用户多次买入同一只股票/基金时，系统会自动合并持仓并计算平均成本价
                var transaction = new InvestmentTransaction
                {
                    InvestmentId = existingInvestment.Id,
                    TransactionType = "buy",
                    Quantity = request.Quantity,
                    Price = request.CostPrice, // 使用用户输入的成本价作为买入价格
                    Amount = request.Quantity * request.CostPrice,
                    Fee = 0, // 默认手续费为0，用户可以在交易记录中手动添加
                    TransactionDate = DateTime.Now,
                    Notes = request.Notes ?? "通过新增投资自动创建",
                    CreatedAt = DateTime.Now
                };

                _context.InvestmentTransactions.Add(transaction);

                // 更新持仓：自动计算平均成本价
                var totalCost = existingInvestment.TotalCost + transaction.Amount + transaction.Fee;
                var totalQuantity = existingInvestment.Quantity + transaction.Quantity;
                existingInvestment.Quantity = totalQuantity;
                existingInvestment.CostPrice = totalQuantity > 0 ? totalCost / totalQuantity : 0;
                existingInvestment.TotalCost = totalCost;

                // 更新当前价格和市值
                // 如果请求中提供了当前价格，使用请求中的价格；否则自动获取
                decimal updatedCurrentPrice;
                if (request.CurrentPrice.HasValue && request.CurrentPrice.Value > 0)
                {
                    updatedCurrentPrice = request.CurrentPrice.Value;
                }
                else
                {
                    updatedCurrentPrice = await GetCurrentPrice(existingInvestment.Code, existingInvestment.Type);
                }
                existingInvestment.CurrentPrice = updatedCurrentPrice;
                existingInvestment.MarketValue = existingInvestment.Quantity * updatedCurrentPrice;
                existingInvestment.ProfitLoss = existingInvestment.MarketValue - existingInvestment.TotalCost;
                existingInvestment.ProfitRate = existingInvestment.TotalCost > 0 
                    ? (existingInvestment.ProfitLoss / existingInvestment.TotalCost) * 100 
                    : 0;
                existingInvestment.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(ApiResponse.Success(
                    new { 
                        id = existingInvestment.Id, 
                        transactionId = transaction.Id,
                        message = "代码已存在，已自动添加为买入交易，持仓已合并"
                    }, 
                    "已添加买入交易，持仓已自动合并"
                ));
            }

            // 如果请求中提供了当前价格，使用请求中的价格；否则自动获取
            decimal currentPrice;
            if (request.CurrentPrice.HasValue && request.CurrentPrice.Value > 0)
            {
                // 使用手动输入的价格
                currentPrice = request.CurrentPrice.Value;
            }
            else
            {
                // 自动获取价格
                currentPrice = await GetCurrentPrice(request.Code, request.Type);
            }

            var investment = new Investment
            {
                Code = request.Code ?? string.Empty,
                Name = request.Name ?? string.Empty, // 处理可能的 null 值
                Type = request.Type ?? "stock",
                Quantity = request.Quantity,
                CostPrice = request.CostPrice,
                CurrentPrice = currentPrice,
                TotalCost = request.Quantity * request.CostPrice,
                MarketValue = request.Quantity * currentPrice,
                Notes = request.Notes,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            investment.ProfitLoss = investment.MarketValue - investment.TotalCost;
            investment.ProfitRate = investment.TotalCost > 0 
                ? (investment.ProfitLoss / investment.TotalCost) * 100 
                : 0;

            _context.Investments.Add(investment);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new { id = investment.Id }, "创建成功"));
        }
        catch (DbUpdateException dbEx)
        {
            // 捕获数据库唯一约束冲突
            if (dbEx.InnerException != null && 
                dbEx.InnerException.Message.Contains("Duplicate entry") && 
                dbEx.InnerException.Message.Contains("uk_code"))
            {
                return Ok(ApiResponse.Error(
                    $"代码 {request?.Code ?? ""} 已存在，请使用\"编辑\"功能更新现有记录", 
                    400
                ));
            }
            return Ok(ApiResponse.Error($"创建失败: {dbEx.Message}", 500));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"创建失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 更新投资记录
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Update(long id, [FromBody] InvestmentRequest request)
    {
        try
        {
            var investment = await _context.Investments.FindAsync(id);
            if (investment == null)
            {
                return Ok(ApiResponse.Error("未找到", 404));
            }

            // 如果请求中提供了当前价格，使用请求中的价格；否则自动获取
            decimal currentPrice;
            if (request.CurrentPrice.HasValue && request.CurrentPrice.Value > 0)
            {
                // 使用手动输入的价格
                currentPrice = request.CurrentPrice.Value;
            }
            else
            {
                // 自动获取价格
                currentPrice = await GetCurrentPrice(request.Code ?? investment.Code, request.Type ?? investment.Type);
            }

            investment.Name = request.Name ?? investment.Name;
            investment.Quantity = request.Quantity;
            investment.CostPrice = request.CostPrice;
            investment.CurrentPrice = currentPrice;
            investment.TotalCost = investment.Quantity * investment.CostPrice;
            investment.MarketValue = investment.Quantity * currentPrice;
            investment.Notes = request.Notes ?? investment.Notes;
            investment.UpdatedAt = DateTime.Now;

            investment.ProfitLoss = investment.MarketValue - investment.TotalCost;
            investment.ProfitRate = investment.TotalCost > 0 
                ? (investment.ProfitLoss / investment.TotalCost) * 100 
                : 0;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(null, "更新成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"更新失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 添加交易记录
    /// </summary>
    [HttpPost("{id}/transaction")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> AddTransaction(long id, [FromBody] TransactionRequest request)
    {
        try
        {
            var investment = await _context.Investments.FindAsync(id);
            if (investment == null)
            {
                return Ok(ApiResponse.Error("未找到", 404));
            }

            var transaction = new InvestmentTransaction
            {
                InvestmentId = id,
                TransactionType = request.TransactionType,
                Quantity = request.Quantity,
                Price = request.Price,
                Amount = request.Quantity * request.Price,
                Fee = request.Fee,
                TransactionDate = request.TransactionDate ?? DateTime.Now,
                Notes = request.Notes,
                CreatedAt = DateTime.Now
            };

            _context.InvestmentTransactions.Add(transaction);

            // 更新持仓
            if (request.TransactionType == "buy")
            {
                var totalCost = investment.TotalCost + transaction.Amount + transaction.Fee;
                var totalQuantity = investment.Quantity + transaction.Quantity;
                investment.Quantity = totalQuantity;
                investment.CostPrice = totalQuantity > 0 ? totalCost / totalQuantity : 0;
                investment.TotalCost = totalCost;
            }
            else if (request.TransactionType == "sell")
            {
                investment.Quantity = Math.Max(0, investment.Quantity - transaction.Quantity);
                investment.TotalCost = investment.Quantity * investment.CostPrice;
            }

            // 更新当前价格和市值
            if (!string.IsNullOrEmpty(investment.Code) && !string.IsNullOrEmpty(investment.Type))
            {
                investment.CurrentPrice = await GetCurrentPrice(investment.Code, investment.Type);
            }
            investment.MarketValue = investment.Quantity * investment.CurrentPrice;
            investment.ProfitLoss = investment.MarketValue - investment.TotalCost;
            investment.ProfitRate = investment.TotalCost > 0 
                ? (investment.ProfitLoss / investment.TotalCost) * 100 
                : 0;
            investment.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new { id = transaction.Id }, "交易记录添加成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"添加失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取统计数据
    /// </summary>
    [HttpGet("stats")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetStats()
    {
        var investments = await _context.Investments.ToListAsync();

        var totalCost = investments.Sum(i => i.TotalCost);
        var totalMarketValue = investments.Sum(i => i.MarketValue);
        var totalProfitLoss = totalMarketValue - totalCost;
        var totalProfitRate = totalCost > 0 ? (totalProfitLoss / totalCost) * 100 : 0;

        // 按类型统计
        var byType = investments
            .GroupBy(i => i.Type)
            .Select(g => new
            {
                Type = g.Key,
                TypeName = g.Key == "stock" ? "股票" : "基金",
                Count = g.Count(),
                TotalCost = g.Sum(i => i.TotalCost),
                TotalMarketValue = g.Sum(i => i.MarketValue),
                ProfitLoss = g.Sum(i => i.ProfitLoss),
                ProfitRate = g.Sum(i => i.TotalCost) > 0 
                    ? (g.Sum(i => i.ProfitLoss) / g.Sum(i => i.TotalCost)) * 100 
                    : 0
            })
            .ToList();

        // 按盈亏状态统计
        var byProfitStatus = investments
            .GroupBy(i => i.ProfitLoss >= 0 ? "profit" : "loss")
            .Select(g => new
            {
                Status = g.Key,
                StatusName = g.Key == "profit" ? "盈利" : "亏损",
                Count = g.Count(),
                TotalCost = g.Sum(i => i.TotalCost),
                TotalMarketValue = g.Sum(i => i.MarketValue),
                ProfitLoss = g.Sum(i => i.ProfitLoss)
            })
            .ToList();

        // Top 5 持仓（按市值）
        var topByMarketValue = investments
            .OrderByDescending(i => i.MarketValue)
            .Take(5)
            .Select(i => new
            {
                i.Code,
                i.Name,
                i.Type,
                i.MarketValue,
                i.ProfitLoss,
                i.ProfitRate
            })
            .ToList();

        // Top 5 收益（按盈亏）
        var topByProfit = investments
            .OrderByDescending(i => i.ProfitLoss)
            .Take(5)
            .Select(i => new
            {
                i.Code,
                i.Name,
                i.Type,
                i.ProfitLoss,
                i.ProfitRate
            })
            .ToList();

        // Top 5 亏损（按盈亏）
        var topByLoss = investments
            .Where(i => i.ProfitLoss < 0)
            .OrderBy(i => i.ProfitLoss)
            .Take(5)
            .Select(i => new
            {
                i.Code,
                i.Name,
                i.Type,
                i.ProfitLoss,
                i.ProfitRate
            })
            .ToList();

        // 资产分布（按代码）
        var assetDistribution = investments
            .Select(i => new
            {
                i.Code,
                i.Name,
                MarketValue = i.MarketValue,
                Percentage = totalMarketValue > 0 ? (i.MarketValue / totalMarketValue) * 100 : 0
            })
            .OrderByDescending(i => i.MarketValue)
            .ToList();

        return Ok(ApiResponse.Success(new
        {
            TotalCost = totalCost,
            TotalMarketValue = totalMarketValue,
            TotalProfitLoss = totalProfitLoss,
            TotalProfitRate = totalProfitRate,
            TotalCount = investments.Count,
            ByType = byType,
            ByProfitStatus = byProfitStatus,
            TopByMarketValue = topByMarketValue,
            TopByProfit = topByProfit,
            TopByLoss = topByLoss,
            AssetDistribution = assetDistribution
        }));
    }

    /// <summary>
    /// 获取历史统计数据（按时间段）
    /// </summary>
    [HttpGet("stats/history")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetHistoryStats(
        [FromQuery] string? period = "month", // day, week, month, quarter, year
        [FromQuery] int? days = 30)
    {
        try
        {
            // 获取交易记录，按时间段分组统计
            var transactions = await _context.InvestmentTransactions
                .Include(t => t.Investment)
                .OrderBy(t => t.TransactionDate)
                .ToListAsync();

            // 如果没有指定天数，根据 period 设置默认值
            if (!days.HasValue)
            {
                days = period?.ToLower() switch
                {
                    "day" => 7,
                    "week" => 30,
                    "month" => 90,
                    "quarter" => 180,
                    "year" => 365,
                    _ => 30
                };
            }

            var startDate = DateTime.Now.AddDays(-days.Value);
            var filteredTransactions = transactions
                .Where(t => t.TransactionDate >= startDate)
                .ToList();

            // 按日期分组统计
            var dailyStats = filteredTransactions
                .GroupBy(t => t.TransactionDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    BuyAmount = g.Where(t => t.TransactionType == "buy").Sum(t => t.Amount),
                    SellAmount = g.Where(t => t.TransactionType == "sell").Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .OrderBy(s => s.Date)
                .ToList();

            // 计算累计投入
            var cumulativeData = new List<object>();
            decimal cumulativeAmount = 0;

            foreach (var stat in dailyStats)
            {
                cumulativeAmount += stat.BuyAmount - stat.SellAmount;
                cumulativeData.Add(new
                {
                    Date = stat.Date.ToString("yyyy-MM-dd"),
                    BuyAmount = stat.BuyAmount,
                    SellAmount = stat.SellAmount,
                    NetAmount = stat.BuyAmount - stat.SellAmount,
                    CumulativeAmount = cumulativeAmount,
                    TransactionCount = stat.TransactionCount
                });
            }

            // 获取当前所有投资的市值变化（模拟历史数据）
            var investments = await _context.Investments.ToListAsync();
            var currentStats = new
            {
                TotalCost = investments.Sum(i => i.TotalCost),
                TotalMarketValue = investments.Sum(i => i.MarketValue),
                TotalProfitLoss = investments.Sum(i => i.ProfitLoss),
                TotalProfitRate = investments.Sum(i => i.TotalCost) > 0
                    ? (investments.Sum(i => i.ProfitLoss) / investments.Sum(i => i.TotalCost)) * 100
                    : 0
            };

            return Ok(ApiResponse.Success(new
            {
                Period = period,
                Days = days,
                StartDate = startDate,
                EndDate = DateTime.Now,
                DailyStats = cumulativeData,
                CurrentStats = currentStats
            }));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"获取历史统计失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 删除投资记录
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Delete(long id)
    {
        try
        {
            var investment = await _context.Investments.FindAsync(id);
            if (investment == null)
            {
                return Ok(ApiResponse.Error("未找到", 404));
            }

            // 删除关联的交易记录
            var transactions = await _context.InvestmentTransactions
                .Where(t => t.InvestmentId == id)
                .ToListAsync();
            _context.InvestmentTransactions.RemoveRange(transactions);

            // 删除投资记录
            _context.Investments.Remove(investment);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(null, "删除成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"删除失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 刷新所有价格
    /// </summary>
    [HttpPost("refresh-prices")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> RefreshPrices()
    {
        try
        {
            var investments = await _context.Investments.ToListAsync();

            foreach (var investment in investments)
            {
                if (!string.IsNullOrEmpty(investment.Code) && !string.IsNullOrEmpty(investment.Type))
                {
                    investment.CurrentPrice = await GetCurrentPrice(investment.Code, investment.Type);
                }
                investment.MarketValue = investment.Quantity * investment.CurrentPrice;
                investment.ProfitLoss = investment.MarketValue - investment.TotalCost;
                investment.ProfitRate = investment.TotalCost > 0 
                    ? (investment.ProfitLoss / investment.TotalCost) * 100 
                    : 0;
                investment.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(null, "价格刷新成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"刷新失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取实时价格（调用东方财富 API）
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
            
            // 判断是否为场外基金（00、01、05开头）
            bool isOTCFund = type?.ToLower() == "fund" && 
                            (paddedCode.StartsWith("00") || paddedCode.StartsWith("01") || paddedCode.StartsWith("05"));
            
            if (isOTCFund)
            {
                // 场外基金：从基金详情页面获取最新净值
                return await GetOTCFundPriceFromWeb(code);
            }
            
            // 场内基金或股票：使用原有逻辑
            // 构建 secid 参数
            // A股股票格式：1.000001（1. + 6位代码）
            // 基金格式：
            //   - 上海市场基金（51、52、53、54、55开头）：1.代码（如 1.510300）
            //   - 深圳市场基金（15、16开头）：0.代码（如 0.159915）
            string secid;
            
            if (type?.ToLower() == "fund")
            {
                // 判断基金市场
                if (paddedCode.StartsWith("51") || paddedCode.StartsWith("52") || 
                    paddedCode.StartsWith("53") || paddedCode.StartsWith("54") || 
                    paddedCode.StartsWith("55") || paddedCode.StartsWith("56") ||
                    paddedCode.StartsWith("57") || paddedCode.StartsWith("58") ||
                    paddedCode.StartsWith("59"))
                {
                    // 上海市场基金：1.代码
                    secid = $"1.{paddedCode}";
                }
                else if (paddedCode.StartsWith("15") || paddedCode.StartsWith("16"))
                {
                    // 深圳市场基金：0.代码
                    secid = $"0.{paddedCode}";
                }
                else
                {
                    // 不支持的基金代码，静默处理
                    return 0;
                }
            }
            else
            {
                // 股票：1.代码
                secid = $"1.{paddedCode}";
            }

            // 调用东方财富 API
            // f43: 当前价格
            // f57: 代码
            // f58: 名称
            var url = $"https://push2.eastmoney.com/api/qt/stock/get?secid={secid}&fields=f43,f57,f58";
            
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // API 响应内容（已移除详细日志）
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                
                // 检查响应状态码（rc: 0 表示成功，其他值表示失败）
                if (result.TryGetProperty("rc", out var rcElement))
                {
                    var rc = rcElement.GetInt32();
                    if (rc != 0)
                    {
                        // API 返回错误码（已移除详细日志）
                        return 0;
                    }
                }
                
                // 解析响应
                // 响应格式：{ "rc": 0, "rt": 6, "svr": ..., "lt": ..., "full": 1, "data": { "f43": 价格, ... } }
                // 注意：data 可能是 null，需要检查
                if (result.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object)
                {
                    if (data.TryGetProperty("f43", out var priceElement) && priceElement.ValueKind != JsonValueKind.Null)
                    {
                        var priceStr = priceElement.GetRawText();
                        if (decimal.TryParse(priceStr, out var price) && price > 0)
                        {
                            // 价格单位是分，需要除以100
                            var finalPrice = price / 100;
                            // 解析成功（已移除详细日志）
                            return finalPrice;
                        }
                        else
                        {
                            // 价格解析失败（已移除详细日志）
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            // 记录错误但不抛出异常，返回0让调用方处理
            // 可以在这里添加日志记录
            // 获取价格失败（已移除详细日志）
        }

        // API 调用失败或解析失败，返回 0
        return 0;
    }

    /// <summary>
    /// 根据代码获取名称（调用东方财富 API）
    /// </summary>
    private async Task<string> GetNameFromCode(string? code, string? type)
    {
        if (string.IsNullOrEmpty(code))
        {
            return string.Empty;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            
            var paddedCode = code.PadLeft(6, '0');
            
            // 判断是否为场外基金（00、01、05开头）
            bool isOTCFund = type?.ToLower() == "fund" && 
                            (paddedCode.StartsWith("00") || paddedCode.StartsWith("01") || paddedCode.StartsWith("05"));
            
            if (isOTCFund)
            {
                // 场外基金：尝试从基金详情页面获取名称
                return await GetOTCFundNameFromWeb(code);
            }
            
            // 场内基金或股票：使用原有逻辑
            string secid;
            if (type?.ToLower() == "fund")
            {
                // 判断基金市场
                if (paddedCode.StartsWith("51") || paddedCode.StartsWith("52") || 
                    paddedCode.StartsWith("53") || paddedCode.StartsWith("54") || 
                    paddedCode.StartsWith("55") || paddedCode.StartsWith("56") ||
                    paddedCode.StartsWith("57") || paddedCode.StartsWith("58") ||
                    paddedCode.StartsWith("59"))
                {
                    // 上海市场基金：1.代码
                    secid = $"1.{paddedCode}";
                }
                else if (paddedCode.StartsWith("15") || paddedCode.StartsWith("16"))
                {
                    // 深圳市场基金：0.代码
                    secid = $"0.{paddedCode}";
                }
                else
                {
                    // 不支持的基金代码，静默处理
                    return string.Empty;
                }
            }
            else
            {
                secid = $"1.{paddedCode}";
            }

            // 调用东方财富 API
            var url = $"https://push2.eastmoney.com/api/qt/stock/get?secid={secid}&fields=f43,f57,f58";
            
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // API 响应内容（已移除详细日志）
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                
                // 检查响应状态码（rc: 0 表示成功，其他值表示失败）
                if (result.TryGetProperty("rc", out var rcElement))
                {
                    var rc = rcElement.GetInt32();
                    if (rc != 0)
                    {
                        // API 返回错误码（已移除详细日志）
                        return string.Empty;
                    }
                }
                
                // 解析响应，获取名称（f58）
                // 注意：data 可能是 null，需要检查
                if (result.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object)
                {
                    if (data.TryGetProperty("f58", out var nameElement) && nameElement.ValueKind != JsonValueKind.Null)
                    {
                        var name = nameElement.GetString();
                        if (!string.IsNullOrEmpty(name))
                        {
                            // 解析成功（已移除详细日志）
                            return name;
                        }
                        else
                        {
                            // f58 字段值为空字符串（已移除详细日志）
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            // 获取名称失败（已移除详细日志）
        }

        // API 调用失败或解析失败，返回空字符串
        return string.Empty;
    }

    /// <summary>
    /// 从基金详情页面获取场外基金名称（通过解析 pingzhongdata JS 文件）
    /// </summary>
    private async Task<string> GetOTCFundNameFromWeb(string? code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return string.Empty;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            
            // 场外基金详情页面的 JS 数据文件
            var url = $"https://fund.eastmoney.com/pingzhongdata/{code.PadLeft(6, '0')}.js";
            
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                // 从 JS 文件中提取基金名称
                // 格式通常是：var fS_name = "基金名称";
                var nameMatch = System.Text.RegularExpressions.Regex.Match(
                    content, 
                    @"var\s+fS_name\s*=\s*[""']([^""']+)[""']",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );
                
                if (nameMatch.Success && nameMatch.Groups.Count > 1)
                {
                    var name = nameMatch.Groups[1].Value;
                    if (!string.IsNullOrEmpty(name))
                    {
                        return name;
                    }
                }
                else
                {
                    // 尝试其他可能的格式
                    nameMatch = System.Text.RegularExpressions.Regex.Match(
                        content,
                        @"fS_name\s*[:=]\s*[""']([^""']+)[""']",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    );
                    if (nameMatch.Success && nameMatch.Groups.Count > 1)
                    {
                        var name = nameMatch.Groups[1].Value;
                        if (!string.IsNullOrEmpty(name))
                        {
                            return name;
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            // 静默处理错误
        }

        return string.Empty;
    }

    /// <summary>
    /// 从基金净值查询 API 获取场外基金最新净值
    /// </summary>
    private async Task<decimal> GetOTCFundPriceFromWeb(string? code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return 0;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(15);
            // 添加 User-Agent，避免被反爬虫拦截
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            
            var paddedCode = code.PadLeft(6, '0');
            
            // 方法1：使用基金实时估值 API（优先获取实时估值，更准确）
            try
            {
                // 使用基金实时估值 API
                var gzUrl = $"https://fundgz.1234567.com.cn/js/{paddedCode}.js?rt={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
                
                var gzResponse = await client.GetAsync(gzUrl);
                if (gzResponse.IsSuccessStatusCode)
                {
                    var gzContent = await gzResponse.Content.ReadAsStringAsync();
                    
                    // 解析 JSONP 响应：jsonpgz({...})
                    var jsonMatch = System.Text.RegularExpressions.Regex.Match(
                        gzContent,
                        @"jsonpgz\((\{.*\})\)",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline
                    );
                    
                    if (jsonMatch.Success && jsonMatch.Groups.Count > 1)
                    {
                        try
                        {
                            var jsonStr = jsonMatch.Groups[1].Value;
                            using var doc = System.Text.Json.JsonDocument.Parse(jsonStr);
                            
                            // 获取实时估值和最新净值
                            decimal? gszValue = null;
                            decimal? dwjzValue = null;
                            
                            // 尝试获取 gsz（实时估值）
                            if (doc.RootElement.TryGetProperty("gsz", out var gsz))
                            {
                                string? priceStr = null;
                                if (gsz.ValueKind == System.Text.Json.JsonValueKind.String)
                                {
                                    priceStr = gsz.GetString();
                                }
                                else if (gsz.ValueKind == System.Text.Json.JsonValueKind.Number)
                                {
                                    priceStr = gsz.GetRawText();
                                }
                                
                                if (!string.IsNullOrEmpty(priceStr) && decimal.TryParse(priceStr, out var price) && price > 0)
                                {
                                    gszValue = price;
                                }
                            }
                            
                            // 尝试获取 dwjz（最新净值）
                            if (doc.RootElement.TryGetProperty("dwjz", out var dwjz))
                            {
                                string? priceStr = null;
                                if (dwjz.ValueKind == System.Text.Json.JsonValueKind.String)
                                {
                                    priceStr = dwjz.GetString();
                                }
                                else if (dwjz.ValueKind == System.Text.Json.JsonValueKind.Number)
                                {
                                    priceStr = dwjz.GetRawText();
                                }
                                
                                if (!string.IsNullOrEmpty(priceStr) && decimal.TryParse(priceStr, out var price) && price > 0)
                                {
                                    dwjzValue = price;
                                }
                            }
                            
                            // 优先使用实时估值（gsz），因为支付宝等平台显示的是实时估值，更接近实际市值
                            // 最新净值（dwjz）是历史确认净值，可能不够及时
                            if (gszValue.HasValue && gszValue.Value > 0)
                            {
                                return gszValue.Value;
                            }
                            
                            // 如果没有实时估值，使用最新净值
                            if (dwjzValue.HasValue && dwjzValue.Value > 0)
                            {
                                return dwjzValue.Value;
                            }
                        }
                        catch (Exception)
                        {
                            // 静默处理 JSON 解析错误
                        }
                    }
                }
            }
            catch (Exception)
            {
                // 静默处理错误
            }
            
            // 方法1.5：使用基金净值查询 API（获取确认净值，验证日期）
            try
            {
                // 使用基金净值查询 API，获取最新确认净值（不是估值）
                var url = $"https://fund.eastmoney.com/f10/F10DataApi.aspx?type=lsjz&code={paddedCode}&page=1&per=1&sdate=&edate=";
                
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    
                    // 解析返回的 JavaScript 对象
                    var contentMatch = System.Text.RegularExpressions.Regex.Match(
                        content,
                        @"content\s*:\s*""([^""]+)""",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    );
                    
                    if (contentMatch.Success && contentMatch.Groups.Count > 1)
                    {
                        var htmlContent = contentMatch.Groups[1].Value;
                        // HTML 转义处理
                        htmlContent = htmlContent.Replace("\\\"", "\"").Replace("\\/", "/").Replace("\\n", "\n").Replace("\\r", "\r");
                        
                        // 先提取日期，验证是否为最新数据
                        var dateMatch = System.Text.RegularExpressions.Regex.Match(
                            htmlContent,
                            @"<tr[^>]*>.*?<td[^>]*>(\d{4}-\d{2}-\d{2})</td>",
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline
                        );
                        
                        if (dateMatch.Success && dateMatch.Groups.Count > 1)
                        {
                            var dateStr = dateMatch.Groups[1].Value;
                            
                            if (DateTime.TryParse(dateStr, out var netWorthDate))
                            {
                                var daysDiff = (DateTime.Now.Date - netWorthDate.Date).Days;
                                
                                // 如果日期超过3天，说明数据可能过旧，不采用
                                if (daysDiff <= 3)
                                {
                                    // 从 HTML 表格中提取单位净值（第一个 tor bold 类的 td）
                                    var priceMatches = System.Text.RegularExpressions.Regex.Matches(
                                        htmlContent,
                                        @"<td[^>]*class=['""]tor bold['""][^>]*>([\d.]+)</td>"
                                    );
                                    
                                    if (priceMatches.Count >= 1)
                                    {
                                        var priceStr = priceMatches[0].Groups[1].Value;
                                        if (decimal.TryParse(priceStr, out var price) && price > 0)
                                        {
                                            return price;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // 静默处理错误
            }
            
            // 方法2：使用基金净值查询 API（备用，获取历史确认净值）
            try
            {
                // 使用基金净值查询 API，获取最新确认净值
                var url = $"https://fund.eastmoney.com/f10/F10DataApi.aspx?type=lsjz&code={paddedCode}&page=1&per=1&sdate=&edate=";
                
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    
                    // 解析返回的 JavaScript 对象
                    var contentMatch = System.Text.RegularExpressions.Regex.Match(
                        content,
                        @"content\s*:\s*""([^""]+)""",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    );
                    
                    if (contentMatch.Success && contentMatch.Groups.Count > 1)
                    {
                        var htmlContent = contentMatch.Groups[1].Value;
                        // HTML 转义处理
                        htmlContent = htmlContent.Replace("\\\"", "\"").Replace("\\/", "/").Replace("\\n", "\n").Replace("\\r", "\r");
                        
                        // 从 HTML 表格中提取单位净值（第一个 tor bold 类的 td）
                        var priceMatches = System.Text.RegularExpressions.Regex.Matches(
                            htmlContent,
                            @"<td[^>]*class=['""]tor bold['""][^>]*>([\d.]+)</td>"
                        );
                        
                                    if (priceMatches.Count >= 1)
                                    {
                                        var priceStr = priceMatches[0].Groups[1].Value;
                                        if (decimal.TryParse(priceStr, out var price) && price > 0)
                                        {
                                            return price;
                                        }
                                    }
                    }
                }
                
                // 备用：使用基金JS数据文件
                var jsUrl = $"https://fund.eastmoney.com/js/{paddedCode}.js";
                
                var jsResponse = await client.GetAsync(jsUrl);
                if (jsResponse.IsSuccessStatusCode)
                {
                    var apiContent = await jsResponse.Content.ReadAsStringAsync();
                    
                    // 从 JS 文件中提取最新净值（估值）
                    // 格式：var fS_gz = "1.4012"; 或 var fS_gz = 1.4012;
                    var gzMatch = System.Text.RegularExpressions.Regex.Match(
                        apiContent,
                        @"var\s+fS_gz\s*=\s*[""']?([\d.]+)[""']?",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    );
                    
                    if (gzMatch.Success && gzMatch.Groups.Count > 1)
                    {
                        var priceStr = gzMatch.Groups[1].Value;
                        if (decimal.TryParse(priceStr, out var price) && price > 0)
                        {
                            return price;
                        }
                    }
                    
                    // 备用：从净值趋势数据中提取最新净值
                    // 格式：var Data_netWorthTrend = [[日期,净值], ...]
                    var trendMatch = System.Text.RegularExpressions.Regex.Match(
                        apiContent,
                        @"Data_netWorthTrend\s*=\s*\[([^\]]+)\]",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline
                    );
                    
                    if (trendMatch.Success && trendMatch.Groups.Count > 1)
                    {
                        var trendData = trendMatch.Groups[1].Value;
                        // 提取最后一个数组的第二个值（净值）
                        var lastArrayMatch = System.Text.RegularExpressions.Regex.Matches(
                            trendData,
                            @"\[([^\]]+)\]"
                        );
                        
                        if (lastArrayMatch.Count > 0)
                        {
                            var lastArray = lastArrayMatch[lastArrayMatch.Count - 1].Groups[1].Value;
                            var values = lastArray.Split(',');
                            if (values.Length >= 2)
                            {
                                var priceStr = values[1].Trim().Trim('"', '\'');
                                if (decimal.TryParse(priceStr, out var price) && price > 0)
                                {
                                    return price;
                                }
                            }
                        }
                    }
                    
                    // 备用：从 pingzhongdata JS 文件获取
                    var pingzhongUrl = $"https://fund.eastmoney.com/pingzhongdata/{paddedCode}.js";
                    var pingzhongResponse = await client.GetAsync(pingzhongUrl);
                    if (pingzhongResponse.IsSuccessStatusCode)
                    {
                        var pingzhongContent = await pingzhongResponse.Content.ReadAsStringAsync();
                        var pingzhongGzMatch = System.Text.RegularExpressions.Regex.Match(
                            pingzhongContent,
                            @"var\s+fS_gz\s*=\s*[""']?([\d.]+)[""']?",
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase
                        );
                        
                        if (pingzhongGzMatch.Success && pingzhongGzMatch.Groups.Count > 1)
                        {
                            var priceStr = pingzhongGzMatch.Groups[1].Value;
                            if (decimal.TryParse(priceStr, out var price) && price > 0)
                            {
                                return price;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // 静默处理错误
            }
            
            // 方法3：使用基金详情页 API（最后备用方案）
            try
            {
                var detailUrl = $"https://fund.eastmoney.com/{paddedCode}.html";
                
                var detailResponse = await client.GetAsync(detailUrl);
                if (detailResponse.IsSuccessStatusCode)
                {
                    var detailContent = await detailResponse.Content.ReadAsStringAsync();
                    
                    // 从详情页提取净值
                    var detailPriceMatch = System.Text.RegularExpressions.Regex.Match(
                        detailContent,
                        @"单位净值[^<]*<span[^>]*>([\d.]+)</span>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    );
                    
                    if (detailPriceMatch.Success && detailPriceMatch.Groups.Count > 1)
                    {
                        var priceStr = detailPriceMatch.Groups[1].Value;
                        if (decimal.TryParse(priceStr, out var price) && price > 0)
                        {
                            return price;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // 静默处理错误
            }
        }
        catch (Exception)
        {
            // 静默处理错误
        }

        return 0;
    }
}

public class InvestmentRequest
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal CostPrice { get; set; }
    public decimal? CurrentPrice { get; set; } // 可选：手动指定的当前价格
    public string? Notes { get; set; }
}

public class TransactionRequest
{
    public string TransactionType { get; set; } = string.Empty; // buy / sell
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Fee { get; set; } = 0;
    public DateTime? TransactionDate { get; set; }
    public string? Notes { get; set; }
}

