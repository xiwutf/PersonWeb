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
            // 记录请求参数，用于调试
            Console.WriteLine($"[AutoFill] 收到请求: code={code}, type={type}");

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(type))
            {
                Console.WriteLine($"[AutoFill] 参数验证失败: code={code}, type={type}");
                return BadRequest(ApiResponse.Error("代码和类型不能为空", 400));
            }

            Console.WriteLine($"[AutoFill] 开始获取价格和名称: code={code}, type={type}");
            var price = await GetCurrentPrice(code, type);
            var name = await GetNameFromCode(code, type);
            Console.WriteLine($"[AutoFill] 获取结果: name={name}, price={price}");

            // 如果名称和价格都为空，说明代码可能不存在或格式不对
            if (string.IsNullOrEmpty(name) && price == 0)
            {
                Console.WriteLine($"[AutoFill] 警告：代码 {code} 无法获取数据，可能不存在或格式不正确");
            }

            return Ok(ApiResponse.Success(new
            {
                Name = name ?? string.Empty,
                CurrentPrice = price
            }));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AutoFill] 异常: {ex.Message}");
            Console.WriteLine($"[AutoFill] 堆栈: {ex.StackTrace}");
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
                var updatedCurrentPrice = await GetCurrentPrice(existingInvestment.Code, existingInvestment.Type);
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

            var currentPrice = await GetCurrentPrice(request.Code, request.Type);

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

            var currentPrice = await GetCurrentPrice(request.Code ?? investment.Code, request.Type ?? investment.Type);

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
                    Console.WriteLine($"[GetCurrentPrice] 不支持的基金代码: {code}");
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
                Console.WriteLine($"[GetCurrentPrice] API 响应内容: {content}");
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                
                // 检查响应状态码（rc: 0 表示成功，其他值表示失败）
                if (result.TryGetProperty("rc", out var rcElement))
                {
                    var rc = rcElement.GetInt32();
                    if (rc != 0)
                    {
                        Console.WriteLine($"[GetCurrentPrice] API 返回错误码: rc={rc}, code={code}, secid={secid}");
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
                            Console.WriteLine($"[GetCurrentPrice] 解析成功: {code} -> {finalPrice} (原始值: {price})");
                            return finalPrice;
                        }
                        else
                        {
                            Console.WriteLine($"[GetCurrentPrice] 价格解析失败: priceStr={priceStr}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[GetCurrentPrice] f43 字段不存在或为 null");
                    }
                }
                else
                {
                    Console.WriteLine($"[GetCurrentPrice] data 字段为空或不是对象: ValueKind={(result.TryGetProperty("data", out var d) ? d.ValueKind.ToString() : "不存在")}");
                }
            }
            else
            {
                Console.WriteLine($"[GetCurrentPrice] HTTP 请求失败: StatusCode={response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            // 记录错误但不抛出异常，返回0让调用方处理
            // 可以在这里添加日志记录
            Console.WriteLine($"获取价格失败 {code}: {ex.Message}");
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
                    Console.WriteLine($"[GetNameFromCode] 不支持的基金代码: {code}");
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
                Console.WriteLine($"[GetNameFromCode] API 响应内容: {content}");
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                
                // 检查响应状态码（rc: 0 表示成功，其他值表示失败）
                if (result.TryGetProperty("rc", out var rcElement))
                {
                    var rc = rcElement.GetInt32();
                    if (rc != 0)
                    {
                        Console.WriteLine($"[GetNameFromCode] API 返回错误码: rc={rc}, code={code}");
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
                            Console.WriteLine($"[GetNameFromCode] 解析成功: {code} -> {name} (secid={secid})");
                            return name;
                        }
                        else
                        {
                            Console.WriteLine($"[GetNameFromCode] f58 字段值为空字符串");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[GetNameFromCode] f58 字段不存在或为 null");
                    }
                }
                else
                {
                    Console.WriteLine($"[GetNameFromCode] data 字段为空或不是对象: ValueKind={(result.TryGetProperty("data", out var d) ? d.ValueKind.ToString() : "不存在")}");
                }
            }
            else
            {
                Console.WriteLine($"[GetNameFromCode] HTTP 请求失败: StatusCode={response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取名称失败 {code}: {ex.Message}");
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
            
            Console.WriteLine($"[GetOTCFundNameFromWeb] 请求场外基金数据: {url}");
            
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
                        Console.WriteLine($"[GetOTCFundNameFromWeb] 解析成功: {code} -> {name}");
                        return name;
                    }
                }
                else
                {
                    Console.WriteLine($"[GetOTCFundNameFromWeb] 未找到基金名称，尝试其他格式");
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
                            Console.WriteLine($"[GetOTCFundNameFromWeb] 解析成功（备用格式）: {code} -> {name}");
                            return name;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"[GetOTCFundNameFromWeb] HTTP 请求失败: StatusCode={response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetOTCFundNameFromWeb] 获取场外基金名称失败 {code}: {ex.Message}");
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
            client.Timeout = TimeSpan.FromSeconds(10);
            
            // 使用基金净值查询 API（返回最新一页数据，per=1 表示只返回1条，即最新净值）
            var url = $"https://fund.eastmoney.com/f10/F10DataApi.aspx?type=lsjz&code={code.PadLeft(6, '0')}&page=1&per=1&sdate=&edate=";
            
            Console.WriteLine($"[GetOTCFundPriceFromWeb] 请求场外基金净值: {url}");
            
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[GetOTCFundPriceFromWeb] API 响应内容（前500字符）: {content.Substring(0, Math.Min(500, content.Length))}");
                
                // 解析返回的 JavaScript 对象
                // 格式：var apidata={ content:"<table>...<td class='tor bold'>1.4078</td>...", ... };
                // 需要从 HTML 表格中提取单位净值（第二个 <td class='tor bold'>）
                var contentMatch = System.Text.RegularExpressions.Regex.Match(
                    content,
                    @"content\s*:\s*""([^""]+)""",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );
                
                if (contentMatch.Success && contentMatch.Groups.Count > 1)
                {
                    var htmlContent = contentMatch.Groups[1].Value;
                    // HTML 转义处理
                    htmlContent = htmlContent.Replace("\\\"", "\"").Replace("\\/", "/");
                    
                    // 从 HTML 表格中提取单位净值
                    // 格式：<td class='tor bold'>1.4078</td>（第二个匹配）
                    var priceMatches = System.Text.RegularExpressions.Regex.Matches(
                        htmlContent,
                        @"<td[^>]*class=['""]tor bold['""][^>]*>([\d.]+)</td>"
                    );
                    
                    if (priceMatches.Count >= 1)
                    {
                        // 第一个匹配是单位净值，第二个是累计净值
                        var priceStr = priceMatches[0].Groups[1].Value;
                        if (decimal.TryParse(priceStr, out var price) && price > 0)
                        {
                            Console.WriteLine($"[GetOTCFundPriceFromWeb] 解析成功: {code} -> {price}");
                            return price;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[GetOTCFundPriceFromWeb] 未找到净值数据，HTML内容: {htmlContent.Substring(0, Math.Min(200, htmlContent.Length))}");
                    }
                }
                else
                {
                    Console.WriteLine($"[GetOTCFundPriceFromWeb] 未找到 content 字段");
                }
            }
            else
            {
                Console.WriteLine($"[GetOTCFundPriceFromWeb] HTTP 请求失败: StatusCode={response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetOTCFundPriceFromWeb] 获取场外基金价格失败 {code}: {ex.Message}");
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

