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

        var byType = investments
            .GroupBy(i => i.Type)
            .Select(g => new
            {
                Type = g.Key,
                Count = g.Count(),
                TotalCost = g.Sum(i => i.TotalCost),
                TotalMarketValue = g.Sum(i => i.MarketValue),
                ProfitLoss = g.Sum(i => i.ProfitLoss)
            })
            .ToList();

        return Ok(ApiResponse.Success(new
        {
            TotalCost = totalCost,
            TotalMarketValue = totalMarketValue,
            TotalProfitLoss = totalProfitLoss,
            TotalProfitRate = totalProfitRate,
            TotalCount = investments.Count,
            ByType = byType
        }));
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
    /// 获取实时价格（模拟，实际需要调用东方财富 API）
    /// </summary>
    private async Task<decimal> GetCurrentPrice(string? code, string? type)
    {
        // TODO: 集成东方财富 API
        // 这里先返回一个模拟价格
        // 实际应该调用：https://push2.eastmoney.com/api/qt/stock/get
        
        try
        {
            // 示例：调用东方财富 API（需要根据实际 API 文档调整）
            var client = _httpClientFactory.CreateClient();
            var url = $"https://push2.eastmoney.com/api/qt/stock/get?secid={code}&fields=f57,f58,f107,f137,f46,f44,f45,f47,f48,f60,f170";
            
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                // 解析价格（根据实际 API 响应格式调整）
                // return result.GetProperty("data").GetProperty("f43").GetDecimal();
            }
        }
        catch
        {
            // 如果 API 调用失败，返回成本价
        }

        // 临时返回随机价格（用于测试）
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

